using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Common.Interfaces.Identity;
using Application.System.Commands.SeedSampleData;
using Domain.Constants.Configuration;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Services.Blob;
using Infrastructure.Services.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            var database = configuration.GetConnectionString(Database.ConnectionStringName);

            services.AddDbContext<JAAADbContext>(options => options.UseSqlServer(database));
            services.AddScoped<IJAAADbContext>(provider => provider.GetService<JAAADbContext>());

            services.AddScoped<IBlobManagerService>(service => new BlobManagerService(storage, container));
            services.AddScoped<IBlobLeaseManager>(service => new BlobLeaseManager(storage, container));

            services.AddScoped<IUserManager, UserManagerService>();
            
            services.DatabaseInitializer<JAAADbContext>();
            services.BlobContainerInitializer();

            return services;
        }

        public async static Task<IServiceCollection> DatabaseInitializer<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            using (var service = services.BuildServiceProvider())
            {
                var appContext = service.GetRequiredService<IJAAADbContext>();
                var dbContext = service.GetRequiredService<TContext>();
                var mediator = service.GetRequiredService<IMediator>();

                dbContext.Database.Migrate();

                var user = await appContext.Users.FirstOrDefaultAsync(u => u.Username == "Default");
                if (user == null)
                {
                    await mediator.Send(new SeedSampleDataCommand());
                }
            }

            return services;
        }

        public async static Task<IServiceCollection> BlobContainerInitializer(this IServiceCollection services)
        {
            using (var service = services.BuildServiceProvider())
            {

            }

            return services;
        }
    }
}
