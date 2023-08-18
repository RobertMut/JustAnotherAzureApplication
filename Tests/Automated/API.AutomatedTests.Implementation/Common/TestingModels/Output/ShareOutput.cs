using System.Diagnostics.CodeAnalysis;

namespace API.AutomatedTests.Implementation.Common.TestingModels.Output;

[ExcludeFromCodeCoverage]
public class ShareOutput
{
    public IEnumerable<Share> Shares { get; set; }
}