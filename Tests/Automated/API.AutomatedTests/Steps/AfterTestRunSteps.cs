using System.Diagnostics;
using API.AutomatedTests.Implementation.Common.Constants;
using API.AutomatedTests.Implementation.Common.Interfaces;
using BoDi;
using TechTalk.SpecFlow;

namespace API.AutomatedTests.Steps;

[Binding]
public static class AfterTestRunSteps
{
    [AfterTestRun(Order = 0)]
    public static void KillProcesses(IObjectContainer objectContainer)
    {
        var apiProcess = objectContainer.Resolve<Process>("api");
        var functionProcess = objectContainer.Resolve<Process>("function");
        var azuriteProcess = objectContainer.Resolve<Process>("azurite");
        
        apiProcess.Kill(true);
        functionProcess.Kill(true);
        azuriteProcess.Kill(true);
    }

    [AfterTestRun(Order = 1)]
    public static async Task ClearDatabase(IObjectContainer objectContainer)
    {
        var commandExecutor = objectContainer.Resolve<ISqlCommandExecutor>();
        var parameters = objectContainer.Resolve<Dictionary<string, string>>(ScenarioContextNames.SqlParameters);
        string query = await File.ReadAllTextAsync("SQLFiles\\ClearDatabase.sql");
        
        await commandExecutor.ExecuteWithoutReturn(query, parameters);
    }
}