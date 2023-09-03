using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Application.GroupShares.Commands.DeleteGroupShare;
using NUnit.Framework;

namespace Application.UnitTests.GroupShares.Commands.DeleteGroupShare;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteGroupShareCommandValidatorTests
{
    private DeleteGroupShareCommandValidator _validator;

    [SetUp]
    public async Task SetUp()
    {
        _validator = new DeleteGroupShareCommandValidator();
    }

    [Test]
    public async Task ValidatorDoesNotThrowException()
    {
        var query = new DeleteGroupShareCommand
        {
            GroupId = Guid.NewGuid().ToString(),
            Filename = "filename"
        };
        
        Assert.DoesNotThrowAsync(async () => await _validator.ValidateAsync(query));
    }
    
    [Test]
    public async Task ValidatorThrowsAtEmptyGroupId()
    {
        var query = new DeleteGroupShareCommand
        {
            Filename = "filename",
            GroupId = "",
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "GroupId must be not empty");
    }
}