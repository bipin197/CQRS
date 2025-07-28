using Amazon.Runtime;
using Amazon.S3;
using ProjectPlanner.Api.Config;
using ProjectPlanner.Api.Middleware;
using ProjectPlanner.Api.Services;
using ProjectPlanner.Commands.Implementations;
using ProjectPlanner.Commands.Interfaces;
using ProjectPlanner.Queries.Implementations;
using ProjectPlanner.Queries.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure AWS Services
var awsSection = builder.Configuration.GetSection("AWS");
var awsOptions = awsSection.Get<AWSOptions>();

var credentials = new BasicAWSCredentials(
    awsOptions?.Credentials?.AccessKey ?? Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"),
    awsOptions?.Credentials?.SecretKey ?? Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY")
);

builder.Services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client(
    credentials,
    new AmazonS3Config
    {
        RegionEndpoint = Amazon.RegionEndpoint.APSouth1,
        ServiceURL = awsOptions?.ServiceURL
    }
));

// Add S3 services
builder.Services.AddSingleton<S3SyncService>();  // Register as singleton first
builder.Services.AddHostedService(sp => sp.GetRequiredService<S3SyncService>()); // Use the same instance as hosted service
builder.Services.AddSingleton<IS3DataSync>(sp => sp.GetRequiredService<S3SyncService>()); // Use the same instance for interface
builder.Services.AddSingleton<S3Storage>();

// Add Memory Cache
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<InMemoryStore>();

// Register CQRS services
builder.Services.AddScoped<IProjectCommands, ProjectCommands>();
builder.Services.AddScoped<IActivityCommands, ActivityCommands>();
builder.Services.AddScoped<IProjectQueries, ProjectQueries>();
builder.Services.AddScoped<IActivityQueries, ActivityQueries>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add Exception Handling Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();
app.Run();
