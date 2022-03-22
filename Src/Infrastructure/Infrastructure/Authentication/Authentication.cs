using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Authentication
{
    public static class Authentication
    {
        public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwt = configuration.GetSection("JWT");
            var container = configuration["ImagesContainer"];

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

            return services;
        }
    }
}
