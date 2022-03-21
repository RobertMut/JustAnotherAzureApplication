using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Common.Interfaces.Identity;
using Application.System.Commands.SeedSampleData;
using Infrastructure.Persistence;
using Infrastructure.Services.Blob;
using Infrastructure.Services.Identity;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var container = configuration["ImagesContainer"];
            var jwt = configuration.GetSection("JWT");

            services.AddDbContext<JAAADbContext>(options => 
            {
                options.UseSqlServer(configuration.GetConnectionString("JAAADatabase"));
            });
            services.AddScoped<IJAAADbContext>(provider => provider.GetService<JAAADbContext>());

            services.AddScoped<IBlobManagerService>(service => new BlobManagerService(configuration.GetValue<string>("AzureWebJobsStorage"), container));
            services.AddScoped<IBlobLeaseManager>(service => new BlobLeaseManager(configuration.GetValue<string>("AzureWebJobsStorage"), container));

            services.AddScoped<IUserManager, UserManagerService>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                string validAudience = jwt.GetValue<string>("ValidAudience");
                string validIssuer = jwt.GetValue<string>("ValidIssuer");
                options.Audience = validAudience;
                options.Authority = validIssuer;
                options.ClaimsIssuer = validIssuer;
                options.RequireHttpsMetadata = false;
                options.Configuration = new OpenIdConnectConfiguration();
                //options.IncludeErrorDetails = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = validAudience,
                    ValidIssuer = validIssuer,
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(MD5.HashData(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JWTSecret"))))
                };
                if (jwt.GetValue<bool>("AllowDangerousCertificate"))
                {
                    options.BackchannelHttpHandler = new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                }
            });

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
