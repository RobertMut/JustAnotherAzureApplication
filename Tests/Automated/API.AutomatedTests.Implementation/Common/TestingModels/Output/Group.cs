using Newtonsoft.Json;

namespace API.AutomatedTests.Implementation.Common.TestingModels.Output;

public class Group
{
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("description")]
    public string Description { get; set; }
}