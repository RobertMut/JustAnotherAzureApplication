using System.Diagnostics.CodeAnalysis;

namespace API.AutomatedTests.Implementation.Common.TestingModels;

[ExcludeFromCodeCoverage]
public class UserShareModel
{
    public string UserId { get; set; }

    public string OtherUserId { get; set; }

    public string Filename { get; set; }
    
    public int Permissions { get; set; }
}