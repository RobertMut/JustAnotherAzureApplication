using Application.Common.Interfaces.Blob;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var storage = configuration.GetConnectionString("Storage");
            var container = configuration["ImagesContainer"];
            services.AddScoped<IBlobManagerService>(service => new BlobManagerService(storage, container));

            return services;
        }
    }
}
