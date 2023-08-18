using System.Diagnostics.CodeAnalysis;
using API.AutomatedTests.Implementation.Common.Constants;
using API.AutomatedTests.Implementation.Common.Interfaces;
using BoDi;
using TechTalk.SpecFlow;

namespace API.AutomatedTests.Implementation.Steps.Helpers;

[ExcludeFromCodeCoverage]
public class DatabaseHelper
{
    public static async Task ClearDatabase(IObjectContainer objectContainer, ISqlCommandExecutor sqlCommandExecutor)
    {
        var sqlParamters = objectContainer.Resolve<Dictionary<string, string>>(ScenarioContextNames.SqlParameters);
        string query = await File.ReadAllTextAsync($"{Files.InitialStringPath}ClearDatabase.sql");

        await sqlCommandExecutor.ExecuteWithoutReturn(query, sqlParamters);
    }
}