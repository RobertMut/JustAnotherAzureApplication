using Application.Common.Interfaces;
using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Functions.UnitTests.Common
{
    public class CustomTestStartup
    {
        public CustomTestStartup()
        {
            Builder = new HostBuilder()
                .ConfigureWebJobs()
                .ConfigureServices(OverrideServices)
                .Build();
        }

        public IHost Builder { get; }

        private void OverrideServices(IServiceCollection services)
        {
            services.AddScoped<IBlobManagerService, MockBlobManagerService>();
            services.AddScoped<ISupportedImageFormats, FunctionImageFormats>();

        }
    }
}
