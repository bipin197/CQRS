using Amazon.S3;
using Amazon.S3.Model;
using ProjectPlanner.Api.Config;
using ProjectPlanner.Commands.Interfaces;
using ProjectPlanner.Domain.Models;
using ProjectPlanner.Queries.Implementations;
using System.Text.Json;

namespace ProjectPlanner.Api.Services
{
    public class S3SyncService : BackgroundService, IS3DataSync
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _activityDataFileName;
        private readonly string _projectDataFileName;
        private readonly InMemoryStore _store;
        private readonly ILogger<S3SyncService> _logger;
        private readonly TimeSpan _syncInterval = TimeSpan.FromMinutes(5);

        public S3SyncService(
            IAmazonS3 s3Client, 
            IConfiguration configuration,
            InMemoryStore store,
            ILogger<S3SyncService> logger)
        {
            _s3Client = s3Client;
            var awsOptions = configuration.GetSection("AWS").Get<AWSOptions>();
            _bucketName = awsOptions?.BucketName ?? throw new ArgumentNullException("BucketName");
            _activityDataFileName = awsOptions?.ActivityDataFileName ?? "Data/ActivityData.json";
            _projectDataFileName = awsOptions?.ProjectDataFileName ?? "Data/ProjectData.json";
            _store = store;
            _logger = logger;
        }

        private readonly SemaphoreSlim _syncLock = new SemaphoreSlim(1, 1);

        public async Task SyncToS3()
        {
            await _syncLock.WaitAsync();
            try
            {
                // Upload activities
                var activitiesJson = JsonSerializer.Serialize(_store.GetAllActivities());
                var activitiesPutRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = _activityDataFileName,
                    ContentType = "application/json",
                    ContentBody = activitiesJson
                };
                await _s3Client.PutObjectAsync(activitiesPutRequest);

                // Upload projects
                var projectsJson = JsonSerializer.Serialize(_store.GetAllProjects());
                var projectsPutRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = _projectDataFileName,
                    ContentType = "application/json",
                    ContentBody = projectsJson
                };
                await _s3Client.PutObjectAsync(projectsPutRequest);

                _logger.LogInformation("Successfully synced data to S3");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing data to S3");
                throw;
            }
            finally
            {
                _syncLock.Release();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Initial sync when service starts
            try
            {
                await SyncProjectsFromS3();
                await SyncActivitiesFromS3();
                _logger.LogInformation("Initial sync completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during initial sync from S3");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_syncInterval, stoppingToken);
                    await SyncProjectsFromS3();
                    await SyncActivitiesFromS3();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error syncing data from S3");
                }
            }
        }

        private async Task SyncProjectsFromS3()
        {
            var projects = new List<Project>();

            // First try to get the single ProjectData.json file
            try
            {
                var getRequest = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = _projectDataFileName
                };

                using var singleFileResponse = await _s3Client.GetObjectAsync(getRequest);
                using var reader = new StreamReader(singleFileResponse.ResponseStream);
                var json = await reader.ReadToEndAsync();
                var projectArray = JsonSerializer.Deserialize<Project[]>(json);
                if (projectArray != null)
                {
                    projects.AddRange(projectArray);
                    _logger.LogInformation($"Successfully loaded {projects.Count} projects from {_projectDataFileName}");
                    _store.UpdateProjects(projects);
                    return;
                }
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation($"{_projectDataFileName} not found, trying projects/ prefix");
            }

            // If single file not found, try the projects/ prefix
            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = "projects/"
            };

            ListObjectsV2Response response;
            do
            {
                response = await _s3Client.ListObjectsV2Async(request);
                foreach (var entry in response.S3Objects)
                {
                    var getRequest = new GetObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = entry.Key
                    };

                    using var response2 = await _s3Client.GetObjectAsync(getRequest);
                    using var reader = new StreamReader(response2.ResponseStream);
                    var json = await reader.ReadToEndAsync();
                    var project = JsonSerializer.Deserialize<Project>(json);
                    if (project != null)
                        projects.Add(project);
                }
                request.ContinuationToken = response.NextContinuationToken;
            }
            while (response.IsTruncated.GetValueOrDefault(true));

            _store.UpdateProjects(projects);
            _logger.LogInformation($"Synced {projects.Count} projects from S3");
        }

        private async Task SyncActivitiesFromS3()
        {
            var activities = new List<Activity>();

            // First try to get the single ActivityData.json file
            try
            {
                var getRequest = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = _activityDataFileName
                };

                using var singleFileResponse = await _s3Client.GetObjectAsync(getRequest);
                using var reader = new StreamReader(singleFileResponse.ResponseStream);
                var json = await reader.ReadToEndAsync();
                var activityArray = JsonSerializer.Deserialize<Activity[]>(json);
                if (activityArray != null)
                {
                    activities.AddRange(activityArray);
                    _logger.LogInformation("Successfully loaded activities from ActivityData.json");
                    _store.UpdateActivities(activities);
                    return;
                }
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("ActivityData.json not found, trying activities/ prefix");
            }

            // If single file not found, try the activities/ prefix
            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = "activities/"
            };

            ListObjectsV2Response response;
            do
            {
                response = await _s3Client.ListObjectsV2Async(request);
                foreach (var entry in response.S3Objects)
                {
                    var getRequest = new GetObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = entry.Key
                    };

                    using var response2 = await _s3Client.GetObjectAsync(getRequest);
                    using var reader = new StreamReader(response2.ResponseStream);
                    var json = await reader.ReadToEndAsync();
                    var activity = JsonSerializer.Deserialize<Activity>(json);
                    if (activity != null)
                        activities.Add(activity);
                }
                request.ContinuationToken = response.NextContinuationToken;
            }
            while (response.IsTruncated.GetValueOrDefault(true));

            _store.UpdateActivities(activities);
            _logger.LogInformation($"Synced {activities.Count} activities from S3");
        }
    }
}