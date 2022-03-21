using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Infrastructure.Persistence;
using Infrastructure.Services.Blob;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace API.IntegrationTests.Common
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseTestServer(x =>
            {
                x.BaseAddress = new Uri("https://localhost:7264/");
            }).UseSetting("https_port", "7264");

            builder.ConfigureServices(services =>
            {
                services.Remove(GetDescriptor<BlobManagerService>(services));
                services.Remove(GetDescriptor<DbContextOptions<JAAADbContext>>(services));

                services.AddScoped<IJAAADbContext, JAAADbContext>();
                services.AddSingleton<IBlobManagerService, MockBlobManagerService>();
                services.AddTransient<IAuthenticationSchemeProvider, MockSchemeProvider>();
                services.AddDbContext<JAAADbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");

                }, ServiceLifetime.Singleton);

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<JAAADbContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    try
                    {
                        var user = db.Users.SingleOrDefault(x => x.Username == "Default");
                        if (user == null)
                        {
                            Utils.InitializeDbForTests(db);
                        }

                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                                            "database with test messages. Error: {Message}", ex.Message);
                    }
                }
            });

            base.ConfigureWebHost(builder);
        }

        private ServiceDescriptor GetDescriptor<TClass>(IServiceCollection services) where TClass : class => services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(TClass));

        public async Task<HttpClient> GetAuthenticatedClient()
        {
            var client = this.CreateClient(this.ClientOptions);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test", "Test");
            return client;
        }
    }
}
