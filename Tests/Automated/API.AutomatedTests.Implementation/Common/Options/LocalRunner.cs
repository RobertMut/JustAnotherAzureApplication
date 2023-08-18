using System.Diagnostics.CodeAnalysis;

namespace API.AutomatedTests.Implementation.Common.Options;

[ExcludeFromCodeCoverage]
public class LocalRunnerOptions
{
    public const string LocalRunner = "LocalRunner";
    public bool CreateNoWindow { get; set; }
}