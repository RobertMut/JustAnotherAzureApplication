using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Application.Account.Commands.Login;
using Application.Account.Commands.Register;
using Application.Common.Models.Account;
using NUnit.Framework;

namespace Application.UnitTests.Accounts.Commands.Register;

[ExcludeFromCodeCoverage]
[TestFixture]
public class RegisterCommandValidatorTests
{
    private RegisterCommandValidator _validator;

    [SetUp]
    public async Task SetUp()
    {
        _validator = new RegisterCommandValidator();
    }

    [Test]
    public async Task ValidatorDoesNotThrowException()
    {
        var query = new RegisterCommand
        {
            RegisterModel = new RegisterModel()
            {
                Username = "Username",
                Password = "Password"
            }
        };
        
        Assert.DoesNotThrowAsync(async () => await _validator.ValidateAsync(query));
    }
    
    [Test]
    public async Task ValidatorThrowsAtEmptyUsername()
    {
        var query = new RegisterCommand
        {
            RegisterModel = new RegisterModel()
            {
                Username = null,
                Password = "Password"
            }
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[1].ErrorMessage == "Username must be not null or empty");
    }
    
    [Test]
    public async Task ValidatorThrowsAtEmptyPassword()
    {
        var query = new RegisterCommand
        {
            RegisterModel = new RegisterModel()
            {
                Username = "Username",
                Password = null
            }
        };

        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[1].ErrorMessage == "Password must be not null or empty");
    }
}