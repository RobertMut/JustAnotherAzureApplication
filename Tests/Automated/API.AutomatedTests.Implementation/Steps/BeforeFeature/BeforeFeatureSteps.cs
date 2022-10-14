using System.Net.Security;
using System.Text;
using API.AutomatedTests.Implementation.Common.Interfaces;
using API.AutomatedTests.Implementation.Common.TestingModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;

namespace API.AutomatedTests.Implementation.Steps.BeforeFeature;

[Binding]
public class BeforeFeatureSteps
{
    private const string EndpointName = "Accounts";
    
    [BeforeFeature("Authenticate", Order = 0)]
    public static async Task AuthenticateToApi(FeatureContext featureContext, IHttpCallerService httpCallerService, IConfiguration configuration)
    {
        string endpoint = configuration.GetSection("Endpoints")[EndpointName];
        var model = new LoginModel
        {
            UserName = "Default",
            Password = "12345"
        };

        var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        var response = await httpCallerService.MakePostCall(endpoint, content, new CancellationToken());
        var json = JObject.Parse(await response.Content.ReadAsStringAsync());
        string token = json.Value<string>("token");

        featureContext.Add("token", token);
    }
}