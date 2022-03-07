using Application.Common.Interfaces;
using Common;
using Infrastructure.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Functions.Startup))]
namespace Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IBlobManagerService>(service =>
            new BlobManagerService(Environment.GetEnvironmentVariable("AzureWebJobsStorage"), Environment.GetEnvironmentVariable("ImagesContainer")));
            builder.Services.AddScoped<ISupportedImageFormats, FunctionImageFormats>();

        }
    }
}
