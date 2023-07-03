using System.Text;
using API.AutomatedTests.Implementation.Common.Constants;
using API.AutomatedTests.Implementation.Common.Interfaces;
using API.AutomatedTests.Implementation.Common.TestingModels;
using API.AutomatedTests.Implementation.Steps.Helpers;
using BoDi;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;
using File = System.IO.File;

namespace API.AutomatedTests.Implementation.Steps.Scenario;

[Binding]
public class BeforeScenarioSteps
{
    private const string InitialStringPath = "SQLFiles\\";
    
    [AfterScenario("CustomData", Order = 0)]
    public static async Task ClearDatabaseFromData(ScenarioContext scenarioContext, ISqlCommandExecutor sqlCommandExecutor, IObjectContainer objectContainer)
    {
        await DatabaseHelper.ClearDatabase(objectContainer, sqlCommandExecutor);
    }
    
    [BeforeScenario("CustomData", Order = 0)]
    public static async Task InsertDataToDatabase(ScenarioContext scenarioContext, ISqlCommandExecutor sqlCommandExecutor, IObjectContainer objectContainer)
    {
        var sqlParamters = objectContainer.Resolve<Dictionary<string, string>>(ScenarioContextNames.SqlParameters);
        string? fileTag = scenarioContext.ScenarioInfo.Tags.FirstOrDefault(x => x.Contains("File:"));

        if (fileTag is null)
        {
            throw new SpecFlowException("Declared custom data for scenario does not contain file tag");
        }

        string sqlFile = fileTag.AsSpan().Slice(5, fileTag.Length-5).ToString();
        string query = await File.ReadAllTextAsync($"{InitialStringPath}{sqlFile}");

        await sqlCommandExecutor.ExecuteWithoutReturn(query, sqlParamters);
    }
}