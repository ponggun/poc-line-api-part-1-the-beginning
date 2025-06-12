using PocLineAPI.Domain.Interfaces;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Services;
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
builder.Services.AddControllersWithViews();

// Configure HttpClient for API communication
builder.Services.AddHttpClient<DocumentApiClient>();

// Register application services
builder.Services.AddScoped<IEmbeddingService, OpenAIEmbeddingService>();
builder.Services.AddScoped<IRepository, QdrantRepository>();
builder.Services.AddScoped<IDocumentService, DocumentService>();

try
{
    Log.Information("Starting web application");
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

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
