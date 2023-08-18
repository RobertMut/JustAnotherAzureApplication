using System.Diagnostics.CodeAnalysis;

namespace API.AutomatedTests.Implementation.Common.TestingModels.Output;

[ExcludeFromCodeCoverage]
public class Share
{
    public string Filename { get; set; }
    public string Permissions { get; set; }
}