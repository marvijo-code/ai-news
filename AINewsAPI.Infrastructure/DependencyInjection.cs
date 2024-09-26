using AINewsAPI.Domain.Interfaces;
using AINewsAPI.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AINewsAPI.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<INewsRepository, NewsRepository>();
            return services;
        }
    }
}
