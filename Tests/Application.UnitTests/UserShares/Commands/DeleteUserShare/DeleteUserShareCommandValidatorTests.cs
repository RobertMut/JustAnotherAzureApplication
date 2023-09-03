using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Application.UserShares.Commands.DeleteUserShare;
using Domain.Enums.Image;
using NUnit.Framework;

namespace Application.UnitTests.UserShares.Commands.DeleteUserShare;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteUserShareCommandValidatorTests
{
    private DeleteUserShareCommandValidator _validator;

    [SetUp]
    public async Task SetUp()
    {
        _validator = new DeleteUserShareCommandValidator();
    }

    [Test]
    public async Task ValidatorDoesNotThrowException()
    {
        var query = new DeleteUserShareCommand
        {
            UserId = Guid.NewGuid().ToString(),
            Filename = "test.png"
        };
        
        Assert.DoesNotThrowAsync(async () => await _validator.ValidateAsync(query));
    }
    
    [Test]
    public async Task ValidatorThrowsAtNullUserId()
    {
        var query = new DeleteUserShareCommand
        {
            Filename = "test.png"
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "UserId must be not null");
    }
    
    [Test]
    public async Task ValidatorThrowsAtEmptyFilename()
    {
        var query = new DeleteUserShareCommand
        {
            UserId = Guid.NewGuid().ToString(),
            Filename = string.Empty
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Filename must be not empty");
    }
}