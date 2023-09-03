using System.Diagnostics.CodeAnalysis;

namespace API.AutomatedTests.Implementation.Common.TestingModels;

[ExcludeFromCodeCoverage]
public class LoginModel
{
    public string UserName { get; set; }
    
    public string Password { get; set; }
}