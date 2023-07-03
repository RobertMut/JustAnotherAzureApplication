using Newtonsoft.Json;

namespace API.AutomatedTests.Implementation.Common.TestingModels.Output;

public class GroupsOutput
{
    [JsonProperty("groups")]
    public IEnumerable<Group> Groups { get; set; }
}