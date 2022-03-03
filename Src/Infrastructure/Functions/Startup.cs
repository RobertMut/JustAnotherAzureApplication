using Application.Common.Interfaces;
using Infrastructure.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Functions.Startup))]
namespace Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IBlobManagerService>(service => 
            new BlobManagerService("DefaultEndpointsProtocol=https;AccountName=jaaastorage;AccountKey=l1XveR1poEtoV9WEeXbuzYqPdLQyThEXVrCh8tWx8Fp4n5qfK/9rG9cnD2DzzlifQsu7/kNvDw+Z+AStz2PSMw==;BlobEndpoint=https://jaaastorage.blob.core.windows.net/;TableEndpoint=https://jaaastorage.table.core.windows.net/;QueueEndpoint=https://jaaastorage.queue.core.windows.net/;FileEndpoint=https://jaaastorage.file.core.windows.net/", "jaaablob"));
            
        }
    }
}
