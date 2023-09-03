using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Application.Account.Commands.Login;
using Application.Common.Models.Account;
using NUnit.Framework;

namespace Application.UnitTests.Accounts.Commands.Login;

[ExcludeFromCodeCoverage]
[TestFixture]
public class LoginCommandValidatorTests
{
    private LoginCommandValidator _validator;

    [SetUp]
    public async Task SetUp()
    {
        _validator = new LoginCommandValidator();
    }

    [Test]
    public async Task ValidatorDoesNotThrowException()
    {
        var query = new LoginCommand
        {
            LoginModel = new LoginModel
            {
                UserName = "Username",
                Password = "Password"
            }
        };
        
        Assert.DoesNotThrowAsync(async () => await _validator.ValidateAsync(query));
    }
    
    [Test]
    public async Task ValidatorThrowsAtEmptyUsername()
    {
        var query = new LoginCommand
        {
            LoginModel = new LoginModel
            {
                UserName = null,
                Password = "Password"
            }
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[1].ErrorMessage == "Username must be not null or empty");
    }
    
    [Test]
    public async Task ValidatorThrowsAtEmptyPassword()
    {
        var query = new LoginCommand
        {
            LoginModel = new LoginModel
            {
                UserName = "Username",
                Password = null
            }
        };

        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[1].ErrorMessage == "Password must be not null or empty");
    }
}