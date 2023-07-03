using API.AutomatedTests.Implementation.Common.Options;
using BoDi;
using Microsoft.Extensions.Configuration;

namespace API.AutomatedTests.Implementation;

public static class DependencyInjection
{
    public static IObjectContainer AddImplementation(this IObjectContainer objectContainer)
    {
        var config = objectContainer.Resolve<IConfiguration>();
        objectContainer.RegisterInstanceAs(config.GetSection(LocalRunnerOptions.LocalRunner).Get<LocalRunnerOptions>());
        return objectContainer;
    }
}