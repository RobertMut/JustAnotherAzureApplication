using System.Diagnostics.CodeAnalysis;
using System.Text;
using API.AutomatedTests.Implementation.Common.Constants;
using API.AutomatedTests.Implementation.Common.Interfaces;
using BoDi;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace API.AutomatedTests.Implementation.Steps;

[ExcludeFromCodeCoverage]
[Binding]
public class DatabaseSteps
{
    private readonly ScenarioContext _scenarioContext;
    private const string InitialStringPath = "SQLFiles\\";
    private readonly ISqlCommandExecutor _sqlCommandExecutor;
    private readonly Dictionary<string, string> _sqlParamters;

    public DatabaseSteps(IObjectContainer objectContainer, ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _sqlParamters = objectContainer.Resolve<Dictionary<string, string>>(ScenarioContextNames.SqlParameters);
        _sqlCommandExecutor = objectContainer.Resolve<ISqlCommandExecutor>();
    }
    
    #region Given

    [Given("I clear the database data")]
    public async Task ClearTheDatabaseData()
    {
        string query = await File.ReadAllTextAsync($"{InitialStringPath}ClearDatabase.sql");

        await _sqlCommandExecutor.ExecuteWithoutReturn(query, _sqlParamters);
    }
    
    [Given("I add (.*) as sql parameter under key (.*)")]
    public async Task AddAsSqlParameter(string parameter, string key)
    {
        _sqlParamters[key] = parameter;

        _scenarioContext.Remove(ScenarioContextNames.SqlParameters);
        _scenarioContext.Add(ScenarioContextNames.SqlParameters, _sqlParamters);
    }
    
    [Given("I save response returned by sql file (.*) under (.*) (and as sql param?)")]
    public async Task SaveResponseReturnedBySqlFileUnderKey(string file, string key, string saveType)
    {
        string query = await File.ReadAllTextAsync($"{InitialStringPath}{file}");

        if (string.IsNullOrEmpty(query))
        {
            throw new SpecFlowException($"{InitialStringPath}{file} returned empty or null query.");
        }
        
        object? response = await _sqlCommandExecutor.ExecuteAndReturnData(query, _sqlParamters);

        if (response is null)
        {
            throw new SpecFlowException($"Response returned by sql file {file} is null.");
        }
        
        _scenarioContext.Add(key, response);

        if (saveType == "and as sql param")
        {
            _sqlParamters[key] = response.ToString();
        }
    }
    #endregion
    
    #region Then

    [Then("Database (not contains|contains) (user|group|group share|user share) '(.*)'")]
    public async Task DatabaseContainsUser(string contains, string type, string user)
    {
        string query = null;
        
        if (type == "user")
        {
             query  = await File.ReadAllTextAsync($"{InitialStringPath}CheckIfUserExistsByItsName.sql", Encoding.UTF8);
        
            _sqlParamters["@username"] = user;
        }

        if (type == "group")
        {
            query  = await File.ReadAllTextAsync($"{InitialStringPath}CheckIfGroupExistsByItsName.sql", Encoding.UTF8);
        
            _sqlParamters["@groupName"] = user;
        }

        if (type == "user share")
        {
            query  = await File.ReadAllTextAsync($"{InitialStringPath}CheckIfUserShareExistsByFilenameAndUserId.sql", Encoding.UTF8);
        
            _sqlParamters["@userShare"] = user;
        }

        if (type == "group share")
        {
            query  = await File.ReadAllTextAsync($"{InitialStringPath}CheckIfGroupShareExistsByFilenameAndGroupId.sql", Encoding.UTF8);
        
            _sqlParamters["@groupShare"] = user;
        }

        if (string.IsNullOrEmpty(query))
        {
            throw new SpecFlowException("File with query is null or does not exist.");
        }
        
        object? response = await _sqlCommandExecutor.ExecuteAndReturnData(query, _sqlParamters);
        
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