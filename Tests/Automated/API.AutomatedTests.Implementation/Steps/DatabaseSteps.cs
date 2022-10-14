using System.Text;
using API.AutomatedTests.Implementation.Common.Interfaces;
using BoDi;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace API.AutomatedTests.Implementation.Steps;

[Binding]
public class DatabaseSteps
{
    private const string InitialStringPath = "SQLFiles\\";
    private readonly ISqlCommandExecutor sqlCommandExecutor;
    private readonly Dictionary<string, string> sqlParamters;

    public DatabaseSteps(IObjectContainer objectContainer)
    {
        sqlParamters = objectContainer.Resolve<Dictionary<string, string>>("sqlParameter");
        sqlCommandExecutor = objectContainer.Resolve<ISqlCommandExecutor>();
    }
    
    #region Given

    [Given("I clear the database data")]
    public async Task ClearTheDatabaseData()
    {
        string query = await File.ReadAllTextAsync($"{InitialStringPath}ClearDatabase.sql");

        await sqlCommandExecutor.ExecuteWithoutReturn(query, sqlParamters);
    }
    
    #endregion
    
    #region Then

    [Then("Database (not contains|contains) user '(.*)'")]
    public async Task DatabaseContainsUser(string contains, string user)
    {
        string query = await File.ReadAllTextAsync($"{InitialStringPath}CheckIfUserExistsByItsName.sql", Encoding.UTF8);
        
        sqlParamters["@username"] = user;
        object? response = await sqlCommandExecutor.ExecuteAndReturnData(query, sqlParamters);
        
        string stringifiedResposne = response?.ToString() ?? "";

        if (contains.Contains("not"))
        {
            stringifiedResposne.Should().Be("NotExists");
        }
        else
        {
            stringifiedResposne.Should().Be("Exists");
        }
        
    }
    
    #endregion
}