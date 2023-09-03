using System.Diagnostics.CodeAnalysis;

namespace API.AutomatedTests.Implementation.Common.TestingModels;

[ExcludeFromCodeCoverage]
public class GroupShareModel
{
    public string GroupId { get; set; }

    public string UserId { get; set; }

    public string Filename { get; set; }
    
    public int Permissions { get; set; }
}