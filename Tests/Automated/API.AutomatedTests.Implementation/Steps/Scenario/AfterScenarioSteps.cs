using System.Diagnostics.CodeAnalysis;
using API.AutomatedTests.Implementation.Common.Constants;
using API.AutomatedTests.Implementation.Common.Interfaces;
using API.AutomatedTests.Implementation.Steps.Helpers;
using BoDi;
using TechTalk.SpecFlow;

namespace API.AutomatedTests.Implementation.Steps.Scenario;

[ExcludeFromCodeCoverage]
[Binding]
public class AfterScenarioSteps
{
    
    [AfterScenario("CustomData", Order = 0)]
    public static async Task ClearDatabaseFromData(ScenarioContext scenarioContext, ISqlCommandExecutor sqlCommandExecutor, IObjectContainer objectContainer)
    {
        await DatabaseHelper.ClearDatabase(objectContainer, sqlCommandExecutor);
    }
}