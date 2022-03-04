using Application.Common.Interfaces;
using Common;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
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
            services.AddSingleton<IBlobManagerService, MockBlobManagerService>();
            services.AddScoped<ISupportedImageFormats, FunctionImageFormats>();
        }
    }
}
