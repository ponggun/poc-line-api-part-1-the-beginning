using PocLineAPI.Domain.Interfaces;
using PocLineAPI.Application.Interfaces;
using PocLineAPI.Application.Services;
using PocLineAPI.Infrastructure.Repositories;
using PocLineAPI.Infrastructure.Services;
using Serilog;

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
builder.Services.AddScoped<IEmbeddingInfraService, OpenAIEmbeddingInfraService>();
builder.Services.AddScoped<IRepository, QdrantRepository>();
builder.Services.AddScoped<IDocumentBusinessService, DocumentBusinessService>();
builder.Services.AddScoped<ILineMessagingInfraService, LineMessagingInfraService>();
builder.Services.AddScoped<ISoftwareBusinessService, SoftwareBusinessService>();

// Prepare options pattern for configuration file
builder.Services.Configure<PocLineAPI.Application.Models.SoftwareOptions>(builder.Configuration.GetSection("Software"));
builder.Services.Configure<PocLineAPI.Application.Models.LineOptions>(builder.Configuration.GetSection("Line"));

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

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
