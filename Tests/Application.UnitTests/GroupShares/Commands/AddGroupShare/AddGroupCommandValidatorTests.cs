using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Application.GroupShares.Commands.AddGroupShare;
using Domain.Enums.Image;
using NUnit.Framework;

namespace Application.UnitTests.GroupShares.Commands.AddGroupShare;

[ExcludeFromCodeCoverage]
[TestFixture]
public class AddGroupShareCommandValidatorTests
{
    private AddGroupShareCommandValidator _validator;

    [SetUp]
    public async Task SetUp()
    {
        _validator = new AddGroupShareCommandValidator();
    }

    [Test]
    public async Task ValidatorDoesNotThrowException()
    {
        var query = new AddGroupShareCommand
        {
            GroupId = Guid.NewGuid().ToString(),
            Filename = "filename",
            PermissionId = Permissions.full,
            UserId = Guid.NewGuid().ToString()
        };
        
        Assert.DoesNotThrowAsync(async () => await _validator.ValidateAsync(query));
    }
    
    [Test]
    public async Task ValidatorThrowsAtNullGroupId()
    {
        var query = new AddGroupShareCommand
        {
            Filename = "filename",
            PermissionId = Permissions.full,
            UserId = Guid.NewGuid().ToString()
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[1].ErrorMessage == "GroupId must be not empty or null");
    }
    
    [Test]
    public async Task ValidatorThrowsAtNullFilename()
    {
        var query = new AddGroupShareCommand
        {
            GroupId = Guid.NewGuid().ToString(),
            PermissionId = Permissions.full,
            UserId = Guid.NewGuid().ToString()
        };

        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[1].ErrorMessage == "Filename must be not null or empty");
    }
    
    [Test]
    public async Task ValidatorThrowsAtNullPermissionId()
    {
        var query = new AddGroupShareCommand
        {
            GroupId = Guid.NewGuid().ToString(),
            Filename = "filename",
            PermissionId = (Permissions)15,
            UserId = Guid.NewGuid().ToString()
        };

        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "PermissionId not supported");
    }

}