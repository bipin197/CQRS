using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using ProjectPlanner.Api.Middleware;
using ProjectPlanner.Api.Services;
using ProjectPlanner.Commands.Implementations;
using ProjectPlanner.Commands.Interfaces;
using ProjectPlanner.Queries.Implementations;
using ProjectPlanner.Queries.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add AWS S3 client
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddSingleton<S3Storage>();

// Add Memory Cache
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<InMemoryStore>();

// Register CQRS services
builder.Services.AddScoped<IProjectCommands, ProjectCommands>();
builder.Services.AddScoped<IActivityCommands, ActivityCommands>();
builder.Services.AddScoped<IProjectQueries, ProjectQueries>();
builder.Services.AddScoped<IActivityQueries, ActivityQueries>();

// Add Background Service
builder.Services.AddHostedService<S3SyncService>();

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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
