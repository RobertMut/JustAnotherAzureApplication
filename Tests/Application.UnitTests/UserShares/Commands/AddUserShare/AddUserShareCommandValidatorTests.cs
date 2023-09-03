using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Application.UserShares.Commands.AddUserShare;
using Domain.Enums.Image;
using NUnit.Framework;

namespace Application.UnitTests.UserShares.Commands.AddUserShare;

[ExcludeFromCodeCoverage]
[TestFixture]
public class AddUserShareCommandValidatorTests
{
    private AddUserShareCommandValidator _validator;

    [SetUp]
    public async Task SetUp()
    {
        _validator = new AddUserShareCommandValidator();
    }

    [Test]
    public async Task ValidatorDoesNotThrowException()
    {
        var query = new AddUserShareCommand
        {
            UserId = Guid.NewGuid().ToString(),
            OtherUserId = Guid.NewGuid().ToString(),
            Filename = "test.png",
            PermissionId = Permissions.full
        };
        
        Assert.DoesNotThrowAsync(async () => await _validator.ValidateAsync(query));
    }
    
    [Test]
    public async Task ValidatorThrowsAtNullUserId()
    {
        var query = new AddUserShareCommand
        {
            OtherUserId = Guid.NewGuid().ToString(),
            Filename = "test.png",
            PermissionId = Permissions.full
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[1].ErrorMessage == "UserId must be not empty or null");
    }
    
    [Test]
    public async Task ValidatorThrowsAtNullOtherUserId()
    {
        var query = new AddUserShareCommand
        {
            UserId = Guid.NewGuid().ToString(),
            Filename = "test.png",
            PermissionId = Permissions.full
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[1].ErrorMessage == "Other UserId must be not empty or null");
    }
    
    [Test]
    public async Task ValidatorThrowsAtFilenameMustBeNotEmptyOrNull()
    {
        var query = new AddUserShareCommand
        {
            UserId = Guid.NewGuid().ToString(),
            OtherUserId = Guid.NewGuid().ToString(),
            PermissionId = Permissions.full
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[1].ErrorMessage == "Filename must be not empty or null");
    }
    
    [Test]
    public async Task ValidatorThrowsAtNotSupportedPermissionId()
    {
        var query = new AddUserShareCommand
        {
            UserId = Guid.NewGuid().ToString(),
            OtherUserId = Guid.NewGuid().ToString(),
            Filename = "test.png",
            PermissionId = (Permissions)15
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Permission not supported");
    }
}