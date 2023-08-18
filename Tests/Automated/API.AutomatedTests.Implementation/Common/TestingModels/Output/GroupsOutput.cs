using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace API.AutomatedTests.Implementation.Common.TestingModels.Output;

[ExcludeFromCodeCoverage]
public class GroupsOutput
{
    [JsonProperty("groups")]
    public IEnumerable<Group> Groups { get; set; }
}