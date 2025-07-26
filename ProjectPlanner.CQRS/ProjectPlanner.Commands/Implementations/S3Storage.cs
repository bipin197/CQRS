using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace ProjectPlanner.Commands.Implementations
{
    public class S3Storage
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Storage(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _bucketName = configuration["AWS:BucketName"] ?? throw new ArgumentNullException("BucketName");
        }

        public async Task SaveObject<T>(string key, T data)
        {
            var json = JsonSerializer.Serialize(data);
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                ContentBody = json
            };

            await _s3Client.PutObjectAsync(request);
        }

        public async Task DeleteObject(string key)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request);
        }
    }
}