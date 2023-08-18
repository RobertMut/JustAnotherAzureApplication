using System.Diagnostics.CodeAnalysis;
using API.AutomatedTests.Implementation;
using API.AutomatedTests.Implementation.Common.Constants;
using API.AutomatedTests.Implementation.Common.Interfaces;
using API.AutomatedTests.Infrastructure;
using BoDi;
using Microsoft.Extensions.Configuration;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;

namespace API.AutomatedTests.Steps;

[ExcludeFromCodeCoverage]
[Binding]
public static class BeforeTestRunSteps
{
    [BeforeTestRun(Order = 0)]
    public static void GetConfiguration(IObjectContainer objectContainer)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        objectContainer.RegisterInstanceAs<IConfiguration>(config);
    }
    
    [BeforeTestRun(Order = 1)]
    public static void InjectToObjectContainer(IObjectContainer objectContainer, SpecFlowConfiguration specFlowConfiguration)
    {
        objectContainer.AddInfrastructure();
        objectContainer.AddImplementation();
    }

    [BeforeTestRun(Order = 2)]
    public static void GetDatabaseParameters(IObjectContainer objectContainer)
    {
        var configuration = objectContainer.Resolve<IConfiguration>();

        var parameters = new Dictionary<string, string>()
        {
            {"@prefix", configuration["TestDataPrefix"]},
            {"@dbSchema", configuration["DatabaseSchema"]}
        };

        objectContainer.RegisterInstanceAs(parameters, ScenarioContextNames.SqlParameters);
    }

    [BeforeTestRun(Order = 3)]
    public static async Task PrepareDatabase(IObjectContainer objectContainer)
    {
        var sqlExecutor = objectContainer.Resolve<ISqlCommandExecutor>();
        var parameters = objectContainer.Resolve<Dictionary<string, string>>(ScenarioContextNames.SqlParameters);
        string query = await File.ReadAllTextAsync("SQLFiles\\ClearDatabase.sql");

        await sqlExecutor.ExecuteWithoutReturn(query, parameters);
    }
    
    [BeforeTestRun(Order = 4)]
    public static void RunCommands(IObjectContainer objectContainer)
    {
        var commandRunner = objectContainer.Resolve<ICommandRunner>();
        string functionPath = Path.GetFullPath(Files.FunctionLocationDebug, AppDomain.CurrentDomain.BaseDirectory);
        string apiPath = Path.GetFullPath(Files.ApiLocation, AppDomain.CurrentDomain.BaseDirectory);

        var apiProcess = commandRunner.Execute("dotnet run --launch-profile \"Testing\"", apiPath);
        var functionProcess = commandRunner.Execute("func host start --pause-on-error --verbose", functionPath);
        var azuriteProcess = commandRunner.Execute("azurite", AppDomain.CurrentDomain.BaseDirectory);
        
        Thread.Sleep(20000);
        objectContainer.RegisterInstanceAs(azuriteProcess, "azurite");
        objectContainer.RegisterInstanceAs(functionProcess, "function");
        objectContainer.RegisterInstanceAs(apiProcess, "api");
    }
}