using PocLineAPI.Domain.Interfaces;
using PocLineAPI.Application.Interfaces;
using PocLineAPI.Application.Services;
using PocLineAPI.Infrastructure.Repositories;
using PocLineAPI.Infrastructure.Services;
using Serilog;
using PocLineAPI.Presentation.WebApi.Options;


var builder = WebApplication.CreateBuilder(args);

// Configure Serilog to write logs to console and file
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console() 
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 31, // Keep logs for the last 31 days
        rollOnFileSizeLimit: true,
        fileSizeLimitBytes: 10_000_000, // Optional: Limit file size (10MB per file)
        shared: true
    )
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register application services
builder.Services.AddScoped<IEmbeddingService, OpenAIEmbeddingService>();
builder.Services.AddScoped<IRepository, QdrantRepository>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.Configure<SoftwareOptions>(
    builder.Configuration.GetSection("Software"));

try
{
    Log.Information("Starting web API");
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
