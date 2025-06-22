using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PocLineAPI.Application.Services;

namespace PocLineAPI.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SoftwareOptions>(configuration.GetSection("Software"));
        services.Configure<LineOptions>(configuration.GetSection("Line"));
        services.Configure<PostgresOptions>(configuration.GetSection("Postgres"));
        return services;
    }

    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<ISoftwareBusinessService, SoftwareBusinessService>();
        services.AddScoped<IMessagingBusinessService, MessagingBusinessService>();
        services.AddScoped<IDocumentBusinessService, DocumentBusinessService>();
        services.AddScoped<IWebhookEventBusinessService, WebhookEventBusinessService>();
        services.AddScoped<IWebhookResponseBusinessService, WebhookResponseBusinessService>();
        return services;
    }
}
