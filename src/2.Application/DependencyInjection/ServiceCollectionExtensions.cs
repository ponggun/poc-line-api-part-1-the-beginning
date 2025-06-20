using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PocLineAPI.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SoftwareOptions>(configuration.GetSection("Software"));
        services.Configure<LineOptions>(configuration.GetSection("Line"));
        return services;
    }

    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IDocumentBusinessService, DocumentBusinessService>();
        services.AddScoped<ISoftwareBusinessService, SoftwareBusinessService>();
        services.AddScoped<IMessagingBusinessService, MessagingBusinessService>();
        return services;
    }
}
