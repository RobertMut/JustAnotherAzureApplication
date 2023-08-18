using System.Diagnostics.CodeAnalysis;
using API.AutomatedTests.Implementation.Common.Interfaces;
using API.AutomatedTests.Infrastructure.API;
using API.AutomatedTests.Infrastructure.CommandRunner;
using API.AutomatedTests.Infrastructure.Database;
using API.AutomatedTests.Infrastructure.Storage;
using BoDi;

namespace API.AutomatedTests.Infrastructure;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IObjectContainer AddInfrastructure(this IObjectContainer objectContainer)
    {
        objectContainer.RegisterTypeAs<LocalCommandRunner, ICommandRunner>();
        objectContainer.RegisterTypeAs<BlobService, IBlobService>();
        objectContainer.RegisterTypeAs<HttpCallerService, IHttpCallerService>();
        objectContainer.RegisterTypeAs<SqlCommandExecutor, ISqlCommandExecutor>();
        
        return objectContainer;
    }
}