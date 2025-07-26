using Amazon.S3;
using Amazon.S3.Model;
using ProjectPlanner.Domain.Models;
using ProjectPlanner.Queries.Implementations;
using System.Text.Json;

namespace ProjectPlanner.Api.Services
{
    public class S3SyncService : BackgroundService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
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
            _bucketName = configuration["AWS:BucketName"] ?? throw new ArgumentNullException("BucketName");
            _store = store;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SyncProjectsFromS3();
                    await SyncActivitiesFromS3();
                    await Task.Delay(_syncInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error syncing data from S3");
                }
            }
        }

        private async Task SyncProjectsFromS3()
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = "projects/"
            };

            var projects = new List<Project>();
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
            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = "activities/"
            };

            var activities = new List<Activity>();
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