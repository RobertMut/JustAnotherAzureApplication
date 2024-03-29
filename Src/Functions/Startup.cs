﻿using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Common.Interfaces.Image;
using Common.Images;
using Functions.Services;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services.Blob;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

[assembly: FunctionsStartup(typeof(Functions.Startup))]

namespace Functions;

[ExcludeFromCodeCoverage]
public class Startup : FunctionsStartup
{
    public const string Storage = "AzureWebJobsStorage";
    public const string Database = "JAAADatabase";
    public const string ContainerStringSetting = "%ImagesContainer%/";

    /// <summary>
    /// Configures services
    /// </summary>
    /// <param name="builder"><see cref="IFunctionsHostBuilder"/></param>
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddDbContext<JAAADbContext>(options =>
        {
            options.UseSqlServer(Environment.GetEnvironmentVariable(Database));
        });
        builder.Services.AddScoped<IJAAADbContext>(provider => provider.GetService<JAAADbContext>());
        builder.Services.AddSingleton<IBlobManagerService>(
            new BlobManagerService(Environment.GetEnvironmentVariable(Storage),
                Environment.GetEnvironmentVariable("ImagesContainer")));
        builder.Services.AddScoped<ISupportedImageFormats, FunctionImageFormats>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IImageEditor, ImageEditor>();
    }
}