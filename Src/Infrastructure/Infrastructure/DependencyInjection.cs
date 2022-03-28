using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Common.Interfaces.Identity;
using Common;
using Domain.Entities;
using Infrastructure.Authentication;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services.Blob;
using Infrastructure.Services.Identity;
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

            services.AddDbContext<JAAADbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("JAAADatabase"));
            });
            services.AddScoped<IJAAADbContext>(provider => provider.GetService<JAAADbContext>())
                .InitDatabase();

            services.AddScoped<IBlobManagerService>(service => new BlobManagerService(configuration.GetValue<string>("AzureWebJobsStorage"), container));
            services.AddScoped<IBlobLeaseManager>(service => new BlobLeaseManager(configuration.GetValue<string>("AzureWebJobsStorage"), container));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDateTime, MachineTime>();

            services.AddJwtBearerAuthentication(configuration);
            services.AddScoped<ITokenGenerator, TokenGenerator>();

            return services;
        }

        public static async Task InitDatabase(this IServiceCollection services)
        {
            using (var service = services.BuildServiceProvider())
            using (var serviceScope = service.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = service.GetService<IJAAADbContext>();
                db.Database.EnsureCreated();

                var user = await db.Users.FirstOrDefaultAsync(u => u.Username == "Default");
                if (user == null)
                {
                    db.Users.Add(new User
                    {
                        Username = "Default",
                        Password = "12345"
                    });
                }
            }
        }
    }
}
