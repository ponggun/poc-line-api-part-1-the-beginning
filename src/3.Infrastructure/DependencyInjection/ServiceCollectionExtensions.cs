using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PocLineAPI.Application;
using PocLineAPI.Domain;

namespace PocLineAPI.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfraServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind PostgresOptions for IOptions<PostgresOptions> usage elsewhere
        services.Configure<PostgresOptions>(configuration.GetSection("Postgres"));

        // Retrieve PostgresOptions from configuration
        var postgresOptions = configuration.GetSection("Postgres").Get<PostgresOptions>();

        // Retrieve the connection string from PostgresOptions
        var connectionString = postgresOptions?.Connection;

        // Register PostgreSQL database context if a connection string is provided
        if (!string.IsNullOrEmpty(connectionString))
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Register repositories
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IWebhookEventRepository, WebhookEventRepository>();
            services.AddScoped<IWebhookResponseRepository, WebhookResponseRepository>();
        }

        // Register other infra services
        services.AddScoped<IEmbeddingInfraService, OpenAIEmbeddingInfraService>();
        services.AddScoped<ILineMessagingInfraService, LineMessagingInfraService>();

        return services;
    }
}
