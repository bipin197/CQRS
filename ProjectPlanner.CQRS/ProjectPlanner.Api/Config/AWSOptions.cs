namespace ProjectPlanner.Api.Config
{
    public class AWSOptions
    {
        public string Region { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string ServiceURL { get; set; } = string.Empty;
        public string Profile { get; set; } = string.Empty;
        public string ActivityDataFileName { get; set; } = "Data/ActivityData.json";
        public string ProjectDataFileName { get; set; } = "Data/ProjectData.json";
        public AWSCredentials? Credentials { get; set; }
    }

    public class AWSCredentials
    {
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
    }
}
