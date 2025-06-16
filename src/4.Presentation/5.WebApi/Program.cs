using PocLineAPI.Domain.Interfaces;
using PocLineAPI.Application.Interfaces;
using PocLineAPI.Application.Services;
using PocLineAPI.Infrastructure.Repositories;
using PocLineAPI.Infrastructure.Services;
using Serilog;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs", DateTime.Now.ToString("yyyyMMddHHmm"));
Directory.CreateDirectory(logDirectory);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(Path.Combine(logDirectory, "log.txt"), rollingInterval: RollingInterval.Hour)
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
