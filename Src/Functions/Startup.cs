using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Common.Images;
using Infrastructure.Persistence;
using Infrastructure.Services.Blob;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Functions.Startup))]
namespace Functions
{
    public class Startup : FunctionsStartup
    {
        public const string Storage = "StorageConnectionString";
        public const string Database = "JAAADatabase";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddDbContext<JAAADbContext>(options =>
            {
                options.UseSqlServer(Environment.GetEnvironmentVariable(Database));
            });
            builder.Services.AddScoped<IJAAADbContext>(provider => provider.GetService<JAAADbContext>());
            builder.Services.AddSingleton<IBlobManagerService>(service =>
                new BlobManagerService(Environment.GetEnvironmentVariable(Storage), Environment.GetEnvironmentVariable("ImagesContainer")));
            builder.Services.AddScoped<ISupportedImageFormats, FunctionImageFormats>();
        }
    }
}
