using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Common.Interfaces.Identity;
using Application.System.Commands.SeedSampleData;
using Domain.Entities;
using Infrastructure.Authentication;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services.Blob;
using Infrastructure.Services.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using File = Domain.Entities.File;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var jwt = configuration.GetSection("JWT");
            var container = configuration["ImagesContainer"];

            services.DatabaseInitializer<JAAADbContext>(configuration);

            services.AddScoped<IBlobManagerService>(service => new BlobManagerService(configuration.GetValue<string>("AzureWebJobsStorage"), container));
            services.AddScoped<IBlobLeaseManager>(service => new BlobLeaseManager(configuration.GetValue<string>("AzureWebJobsStorage"), container));

            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddScoped<IRepository<File>, FileRepository>();

            services.AddJwtBearerAuthentication(configuration);
            services.AddScoped<ITokenGenerator, TokenGenerator>();

            return services;
        }

        public async static Task<IServiceCollection> DatabaseInitializer<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext
        {
            services.AddDbContext<JAAADbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("JAAADatabase"));
            });
            services.AddScoped<IJAAADbContext>(provider => provider.GetService<JAAADbContext>());

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
    }
}
