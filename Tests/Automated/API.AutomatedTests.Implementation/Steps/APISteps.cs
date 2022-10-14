using System.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using API.AutomatedTests.Implementation.Common.Interfaces;
using BoDi;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace API.AutomatedTests.Implementation.Steps;

[Binding]
public class APISteps
{
    private const string TestingModelsNamespace = "API.AutomatedTests.Implementation.Common.TestingModels";
    private readonly ScenarioContext _scenarioContext;
    private readonly IConfiguration _configuration;
    private readonly IHttpCallerService httpCallerService;
    private readonly string defaultUserToken;

    public APISteps(IObjectContainer objectContainer, FeatureContext featureContext, ScenarioContext scenarioContext, IConfiguration configuration)
    {
        httpCallerService = objectContainer.Resolve<IHttpCallerService>(); 
        _scenarioContext = scenarioContext;
        _configuration = configuration;
        defaultUserToken = featureContext.Get<string>("token");
    }
    
    #region Given

    [Given("I use '(.*)' endpoint")]
    public async Task IUseEndpoint(string endpoint)
    {
        var section = _configuration.GetSection("Endpoints");
        var endpointToBeCalled = section[endpoint];
        
        if (string.IsNullOrEmpty(endpointToBeCalled))
        {
            throw new Exception("Endpoint is null or empty!");
        }
        
        _scenarioContext.Add("endpoint", endpointToBeCalled);
    }

    [Given("I prepare request with following values using '(.*)' model")]
    public async Task IPrepareRequestWithFollowingValuesUsingModel(string model, Table table)
    {
        var foundModel = Assembly.GetExecutingAssembly()
            .GetTypes()
            .FirstOrDefault(t => t.IsClass && t.Namespace.Equals(TestingModelsNamespace));
        
        if (foundModel is null)
        {
            throw new SpecFlowException($"Model {model} was not found!");
        }

        var instance = Activator.CreateInstance(foundModel);
        table.FillInstance(instance);

        var content = new StringContent(JsonConvert.SerializeObject(instance), Encoding.UTF8, "application/json");
        
        _scenarioContext.Add("httpContent", content);
    }

    [Given("I get token from response")]
    public async Task IGetTokenFromResponse()
    {
        var response = _scenarioContext.Get<HttpResponseMessage>("response");
        
        string responseString = await response.Content.ReadAsStringAsync();
        var jsonResponse = JObject.Parse(responseString);
        string token = jsonResponse.Value<string>("token");
        
        _scenarioContext.Add("token", token);
    }

    #endregion
    #region When

    [When("I make call to endpoint(| with an authorization token)")]
    public async Task IMakeCallToEndpoint(string authorization)
    {
        AuthenticationHeaderValue authenticationHeaderValue = null;
        string endpoint = _scenarioContext.Get<string>("endpoint");
        var httpContent = _scenarioContext.Get<HttpContent>("httpContent");

        if (!string.IsNullOrEmpty(authorization))
        {
            authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", this.defaultUserToken);
        }
        
        var response = await httpCallerService.MakePostCall(endpoint, httpContent, new CancellationToken(),
            authenticationHeaderValue);
        
        _scenarioContext.Add("response", response);
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
        
        responseString.Contains(expectedMessage).Should().BeTrue();
    }
    #endregion
}