using Newtonsoft.Json;

namespace API.AutomatedTests.Implementation.Common.TestingModels.Output;

public class FileList {
    [JsonProperty("files")]
    public IEnumerable<File> Files { get; set; }
}