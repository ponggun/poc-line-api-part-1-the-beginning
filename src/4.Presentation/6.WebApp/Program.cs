using PocLineAPI.Domain.Interfaces;
using PocLineAPI.Application.Interfaces;
using PocLineAPI.Application.Services;
using PocLineAPI.Infrastructure.Repositories;
using PocLineAPI.Infrastructure.Services;
using PocLineAPI.Presentation.WebApp.Services;
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
