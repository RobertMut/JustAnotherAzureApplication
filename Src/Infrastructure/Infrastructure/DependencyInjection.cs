using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Common.Interfaces.Identity;
using Domain.Entities;
using Infrastructure.Authentication;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services.Blob;
using Infrastructure.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Adds DBContext, Unit of Work, BlobManagerService, BlobLeaseManager Token Generator
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
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

        services.AddJwtBearerAuthentication(configuration);
        services.AddScoped<ITokenGenerator, TokenGenerator>();

        return services;
    }

    /// <summary>
    /// Inits database
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    public static async Task InitDatabase(this IServiceCollection services)
    {
        using (var service = services.BuildServiceProvider())
        using (var serviceScope = service.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var db = service.GetService<IJAAADbContext>();
            await db.Database.MigrateAsync();

            var group = await db.Groups.FirstOrDefaultAsync(g => g.Name == "Everyone");

            if (group == null)
            {
                db.Users.Add(new User
                {
                    Username = "Default",
                    Password = "12345"
                });

                db.Groups.Add(new Group
                {
                    Name = "Everyone",
                    Description = "Everyone"
                });

                db.Permissions.AddRange(new[]
                {
                    new Permission
                    {
                        Id = 0,
                        Name = "Full",
                        Delete = true,
                        Read = true,
                        Write = true
                    },
                    new Permission
                    {
                        Id = 1,
                        Name = "ReadWrite",
                        Delete = false,
                        Read = true,
                        Write = true
                    },
                    new Permission
                    {
                        Id = 2,
                        Name = "Read",
                        Delete = false,
                        Read = true,
                        Write = false,
                    },
                    new Permission
                    {
                        Id = 3,
                        Name = "Write",
                        Delete = false,
                        Read = false,
                        Write = true
                    }
                });

                await db.SaveChangesAsync();
            }
        }
    }
}