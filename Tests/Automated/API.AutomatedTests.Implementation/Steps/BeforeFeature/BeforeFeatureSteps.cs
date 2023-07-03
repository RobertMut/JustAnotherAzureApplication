using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Text;
using API.AutomatedTests.Implementation.Common.Interfaces;
using API.AutomatedTests.Implementation.Common.TestingModels;
using BoDi;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;
using File = System.IO.File;

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
        var response = await httpCallerService.MakePostCall(endpoint, "POST", content, new CancellationToken());
        var json = JObject.Parse(await response.Content.ReadAsStringAsync());
        string token = json.Value<string>("token");

        featureContext.Add("token", token);
    }
    
    [BeforeFeature("Storage", Order = 1)]
    public static async Task UploadToStorage(FeatureContext featureContext, IBlobService blobService, IConfiguration configuration)
    {
        string filesString = new String((featureContext.FeatureInfo.Tags.FirstOrDefault(x => x.Contains("Files")) 
                            ?? throw new InvalidOperationException("Files were null")).Skip(6).ToArray());
        
        string[] files = filesString.Split(";");
        
        foreach (var file in files)
        {
            string[] names = file.Split(":");
            byte[] fileBytes = await File.ReadAllBytesAsync($"Files\\{names[0]}");
            await blobService.AddBlobWithMetadata(names[1], fileBytes, new CancellationToken());
        }
    }
}