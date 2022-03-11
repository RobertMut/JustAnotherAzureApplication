using Application.Common.Interfaces.Blob;
using Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace API.IntegrationTests.Common
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseTestServer(x =>
            {
                x.BaseAddress = new System.Uri("https://localhost:7264/");
            }).UseSetting("https_port", "7264");
            builder.ConfigureServices(services =>
            {
                services.Remove(services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(BlobManagerService<Azure.Storage.Blobs.BlobServiceClient>)));

                var blobService = new BlobServiceClient();

                services.AddSingleton<IBlobManagerService>(provider => new BlobManagerService<BlobServiceClient>(blobService, ""));
            });

            base.ConfigureWebHost(builder);
        }
    }
}
