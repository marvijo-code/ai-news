using AINewsAPI.Application.Interfaces;
using AINewsAPI.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AINewsAPI.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<INewsService, NewsService>();
            return services;
        }
    }
}
