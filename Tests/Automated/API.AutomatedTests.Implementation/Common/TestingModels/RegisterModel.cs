using System.Diagnostics.CodeAnalysis;

namespace API.AutomatedTests.Implementation.Common.TestingModels;

[ExcludeFromCodeCoverage]
public class RegisterModel
{
    public string Username { get; set; }

    public string Password { get; set; }
}