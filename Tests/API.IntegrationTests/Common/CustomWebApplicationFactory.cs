using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Infrastructure.Persistence;
using Infrastructure.Services.Blob;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace API.IntegrationTests.Common
{
    /// <summary>
    /// Class CustomWebApplicationFactory
    /// </summary>
    /// <typeparam name="TStartup">Startup</typeparam>
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        /// <summary>
        /// Configures test WebHost
        /// </summary>
        /// <param name="builder"><see cref="IWebHostBuilder"/></param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseTestServer(x =>
            {
                x.BaseAddress = new Uri("https://localhost:7264/");
            }).UseSetting("https_port", "7264");
            builder.UseConfiguration(new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .AddEnvironmentVariables()
                .Build());
            
            builder.ConfigureServices(services =>
            {
                services.Remove(GetDescriptor<DbContextOptions<JAAADbContext>>(services));

                services.AddDbContext<JAAADbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");

                });

                services.AddScoped<IJAAADbContext, JAAADbContext>();
                
                services.AddTransient<IAuthenticationSchemeProvider, MockSchemeProvider>();

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<JAAADbContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    try
                    {
                        // var user = db.Users.SingleOrDefault(x => x.Username == "Default");
                        // if (user == null)
                        // {
                        //     Utils.InitializeDbForTests(db);
                        // }

                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                                            "database with test messages. Error: {Message}", ex.Message);
                    }
                }
            });
            
            //base.ConfigureWebHost(builder);
        }

        /// <summary>
        /// Gets service descriptor
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <typeparam name="TClass">Service</typeparam>
        /// <returns><see cref="ServiceDescriptor"/></returns>
        private ServiceDescriptor GetDescriptor<TClass>(IServiceCollection services) where TClass : class => services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(TClass));

        /// <summary>
        /// Gets authenticated client
        /// </summary>
        /// <returns>Authenticated <see cref="HttpClient"/></returns>
        public async Task<HttpClient> GetAuthenticatedClient()
        {
            var client = this.CreateClient(this.ClientOptions);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, "Test");
            return client;
        }
    }
}
