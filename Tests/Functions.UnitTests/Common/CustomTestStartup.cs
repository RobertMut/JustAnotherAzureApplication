using Application.Common.Interfaces.Blob;
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
            var manager = new MockBlobManagerService();

            services.AddSingleton<IBlobManagerService>(provider => manager);
            services.AddSingleton<IBlobCreatorService>(provider => manager);
            services.AddScoped<ISupportedImageFormats, FunctionImageFormats>();
        }
    }
}
