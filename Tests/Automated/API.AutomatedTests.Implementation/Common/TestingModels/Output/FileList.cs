using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace API.AutomatedTests.Implementation.Common.TestingModels.Output;

[ExcludeFromCodeCoverage]
public class FileList {
    [JsonProperty("files")]
    public IEnumerable<File> Files { get; set; }
}