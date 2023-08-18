using System.Diagnostics.CodeAnalysis;

namespace API.AutomatedTests.Implementation.Common.TestingModels;

[ExcludeFromCodeCoverage]
public class CreateGroupModel
{
    /// <summary>
    /// Group name
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Group description
    /// </summary>
    public string Description { get; set; }
}