﻿using System.Diagnostics.CodeAnalysis;
using API.AutomatedTests.Implementation.Common.Options;
using BoDi;
using Microsoft.Extensions.Configuration;

namespace API.AutomatedTests.Implementation;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IObjectContainer AddImplementation(this IObjectContainer objectContainer)
    {
        var config = objectContainer.Resolve<IConfiguration>();
        objectContainer.RegisterInstanceAs(config.GetSection(LocalRunnerOptions.LocalRunner).Get<LocalRunnerOptions>());
        objectContainer.RegisterInstanceAs(config.GetSection(ConnectionStringsOptions.ConnectionStrings).Get<ConnectionStringsOptions>());
        return objectContainer;
    }
}