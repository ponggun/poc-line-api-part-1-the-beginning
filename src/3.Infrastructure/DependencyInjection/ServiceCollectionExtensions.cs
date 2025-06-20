using Microsoft.Extensions.DependencyInjection;
using PocLineAPI.Application;
using PocLineAPI.Domain;

namespace PocLineAPI.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services)
        {
            services.AddScoped<IEmbeddingInfraService, OpenAIEmbeddingInfraService>();
            services.AddScoped<IRepository, QdrantRepository>();
            services.AddScoped<ILineMessagingInfraService, LineMessagingInfraService>();
            return services;
        }
    }
}
