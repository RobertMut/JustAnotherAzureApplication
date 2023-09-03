using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using API.AutomatedTests.Implementation.Common.Constants;
using API.AutomatedTests.Implementation.Common.Interfaces;
using BoDi;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace API.AutomatedTests.Implementation.Steps;

[ExcludeFromCodeCoverage]
[Binding]
public class ApiSteps
{
    private const string TestingModelsNamespace = "API.AutomatedTests.Implementation.Common.TestingModels";
    private readonly ScenarioContext _scenarioContext;
    private readonly IConfiguration _configuration;
    private readonly IObjectContainer _objectContainer;
    private readonly IHttpCallerService _httpCallerService;
    private readonly string _defaultUserToken;

    public ApiSteps(IObjectContainer objectContainer, IHttpCallerService httpCallerService,
        FeatureContext featureContext, ScenarioContext scenarioContext,
        IConfiguration configuration)
    {
        _objectContainer = objectContainer;
        _httpCallerService = httpCallerService;
        _scenarioContext = scenarioContext;
        _configuration = configuration;
        _defaultUserToken = featureContext.Get<string>("token");
    }

    #region Given

    [Given("I use '(.*)' endpoint")]
    public async Task UseEndpoint(string endpoint)
    {
        var section = _configuration.GetSection("Endpoints");
        var endpointToBeCalled = section[endpoint];

        if (string.IsNullOrEmpty(endpointToBeCalled))
        {
            throw new Exception("Endpoint is null or empty!");
        }

        _scenarioContext["endpoint"] = endpointToBeCalled;
    }

    [Given(@"I prepare form from table")]
    public async Task GivenIPrepareFormFromTable(Table table)
    {
        var content = new MultipartFormDataContent();
        var rows = table.Rows[0];
        foreach (var header in table.Header)
        {
            var row = rows[header];

            if (row.StartsWith("File:"))
            {
                string filename = new string(row.Skip(5).ToArray());
                byte[] file = await File.ReadAllBytesAsync($"Files\\{filename}");

                var streamContent = new StreamContent(new MemoryStream(file));
                streamContent.Headers.ContentType =
                    new MediaTypeHeaderValue($"image/{Path.GetExtension(filename).Remove(0, 1)}");

                content.Add(streamContent, header, filename);
            }
            else
            {
                content.Add(new StringContent(row), header);
            }
        }

        _scenarioContext.Add("httpContent", content);
    }

    [Given("I prepare request with following values using '(.*)' model")]
    public async Task PrepareRequestWithFollowingValuesUsingModel(string model, Table table)
    {
        Table replacedTable = new Table(table.Header.ToArray());
        var sqlParameters = _objectContainer.Resolve<Dictionary<string, string>>(ScenarioContextNames.SqlParameters);
        var foundModel = Array.Find(Assembly.GetExecutingAssembly()
            .GetTypes(), t => t.IsClass && t.Namespace.Equals(TestingModelsNamespace) && t.Name == model);

        if (foundModel is null)
        {
            throw new SpecFlowException($"Model {model} was not found!");
        }

        foreach (TableRow row in table.Rows)
        {
            List<string> cells = new List<string>();
            for (int i = 0; i < row.Count; i++)
            {
                var cell = row[i];
                if (cell.StartsWith('@'))
                {
                    cells.Add(sqlParameters[cell]);
                }
                else
                {
                    cells.Add(cell);
                }
            }

            replacedTable.AddRow(cells.ToArray());
        }

        var instance = Activator.CreateInstance(foundModel);
        replacedTable.FillInstance(instance);

        var content = new StringContent(JsonConvert.SerializeObject(instance), Encoding.UTF8, "application/json");

        _scenarioContext.Add("httpContent", content);
    }

    [Given("I prepare request with file '(.*)'")]
    public async Task PrepareRequestWithFileUsingModel(string file)
    {
        string fileContent = await File.ReadAllTextAsync(file);

        if (string.IsNullOrEmpty(fileContent))
        {
            throw new SpecFlowException($"File {file} does not exist or is empty!");
        }

        var content = new StringContent(fileContent, Encoding.UTF8, "application/json");

        _scenarioContext.Add("httpContent", content);
    }

    [Given("I encode url")]
    public async Task EncodeUrl()
    {
        string endpoint = _scenarioContext.Get<string>("endpoint");
        _scenarioContext.Remove("endpoint");
        string[] parts = endpoint.Split("/");
        IEnumerable<string> replacedElements = parts.Skip(2).Select(x => Uri.EscapeDataString(x).Replace("-", "%2d"));

        endpoint = string.Join("/", parts.Take(2).Concat(replacedElements));

        _scenarioContext.Add("endpoint", endpoint);
    }

    [Given("I get token from response")]
    public async Task GetTokenFromResponse()
    {
        var response = _scenarioContext.Get<HttpResponseMessage>("response");

        string responseString = await response.Content.ReadAsStringAsync();
        var jsonResponse = JObject.Parse(responseString);
        string token = jsonResponse.Value<string>("token");

        _scenarioContext.Add("token", token);
    }

    [Given("I replace url parameters with value under key (.*)")]
    public async Task GetTokenFromResponse(string keys)
    {
        string endpoint = _scenarioContext.Get<string>("endpoint");
        string[] keysArray = keys.Split(';');
        object[] values = new object[keysArray.Length];

        for (int i = 0; i < values.Length; i++)
        {
            if (keysArray[i].StartsWith('@'))
            {
                values[i] = _scenarioContext.Get<object>(keysArray[i]);
            }
            else
            {
                values[i] = keysArray[i];
            }
        }

        endpoint = string.Format(endpoint, values);

        _scenarioContext.Remove("endpoint");
        _scenarioContext.Add("endpoint", endpoint);
    }

    [Given("I add '(.*)' as url parameter")]
    public async Task AddUrlParameter(string parameter)
    {
        string endpoint = _scenarioContext.Get<string>("endpoint");

        if (parameter.Contains(";"))
        {
            endpoint = string.Format(endpoint, parameter.Split(";"));
        }
        else
        {
            endpoint = string.Format(endpoint, parameter);
        }

        _scenarioContext.Remove("endpoint");
        _scenarioContext.Add("endpoint", endpoint);
    }

    #endregion

    #region When

    [When("I make call to endpoint(| with an authorization token) using (.*) method")]
    public async Task MakeCallToEndpoint(string authorization, string callMethod)
    {
        AuthenticationHeaderValue authenticationHeaderValue = null;
        string endpoint = _scenarioContext.Get<string>("endpoint");
        _scenarioContext.TryGetValue("httpContent", out HttpContent httpContent);

        if (!string.IsNullOrEmpty(authorization))
        {
            authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", this._defaultUserToken);
        }

        var response = await _httpCallerService.MakePostCall(endpoint, callMethod, httpContent, new CancellationToken(),
            authenticationHeaderValue);

        _scenarioContext["response"] = response;
    }

    #endregion

    #region Then

    [Then("Response code is '(.*)'")]
    public async Task ResponseCodeIs(string response)
    {
        var responseMessage = _scenarioContext.Get<HttpResponseMessage>("response");
        var expectedResponse = Enum.Parse<HttpStatusCode>(response);

        responseMessage.StatusCode.Should().Be(expectedResponse);
    }

    [Then("Response message contains '(.*)'")]
    public async Task ResponseMessageIs(string expectedMessage)
    {
        var responseMessage = _scenarioContext.Get<HttpResponseMessage>("response");
        string responseString = await responseMessage.Content.ReadAsStringAsync();

        responseString.Contains(expectedMessage).Should().BeTrue($"Expected {expectedMessage}, but it was not found in {responseString}");
    }

    [Then("Response file is same as '(.*)'")]
    public async Task ResponseFileIs(string file)
    {
        var responseMessage = _scenarioContext.Get<HttpResponseMessage>("response");
        
        byte[] responseFile = await responseMessage.Content.ReadAsByteArrayAsync();
        byte[] expected = await File.ReadAllBytesAsync($"Files\\{file}");

        responseFile.Should().HaveCount(x => expected.Length - 2 <= x && x <= expected.Length + 2);
    }

    [Then("Response mapped as '(.*)' is the same as '(.*)'")]
    public async Task ResponseIsTheSameAsJson(string model, string jsonFile)
    {
        var responseMessage = _scenarioContext.Get<HttpResponseMessage>("response");
        string apiResponseString = await responseMessage.Content.ReadAsStringAsync();
        string jsonString = await File.ReadAllTextAsync($"{jsonFile}", Encoding.UTF8);
        var type = Type.GetType($"{TestingModelsNamespace}.Output.{model}");

        var response = JsonConvert.DeserializeObject(
            apiResponseString, type);
        var baseModel = JsonConvert.DeserializeObject(
            jsonString, type);

        response.Should().BeEquivalentTo(baseModel);
    }

    #endregion
}