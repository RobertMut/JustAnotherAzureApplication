using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Common.Interfaces.Image;
using Common.Images;
using Domain.Entities;
using Functions.Services;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
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
        public const string Storage = "AzureWebJobsStorage";
        public const string Database = "JAAADatabase";
        public const string ContainerStringSetting = "%ImagesContainer%/";

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
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IImageEditor, ImageEditor>();
        }
    }
}
