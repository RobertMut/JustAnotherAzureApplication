using Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace API.IntegrationTests.Common
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseTestServer();
            builder.ConfigureServices(services =>
            {
                services.Remove(services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(BlobManagerService)));
            });
            base.ConfigureWebHost(builder);
        }
    }
}
