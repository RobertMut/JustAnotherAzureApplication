using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Application.Groups.Commands.UpdateGroup;
using NUnit.Framework;

namespace Application.UnitTests.Groups.Commands.UpdateGroup;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UpdateGroupCommandValidatorTests
{
    private UpdateGroupCommandValidator _validator;

    [SetUp]
    public async Task SetUp()
    {
        _validator = new UpdateGroupCommandValidator();
    }

    [Test]
    public async Task ValidatorDoesNotThrowException()
    {
        var query = new UpdateGroupCommand
        {
            Name = "name",
            Description = "desc"
        };
        
        Assert.DoesNotThrowAsync(async () => await _validator.ValidateAsync(query));
    }
    
    [Test]
    public async Task ValidatorThrowsAtNullName()
    {
        var query = new UpdateGroupCommand
        {
            Name = null,
            Description = "desc"
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[1].ErrorMessage == "Name must be not null or empty");
    }
    
    [Test]
    public async Task ValidatorThrowsAtNullDescription()
    {
        var query = new UpdateGroupCommand
        {
            Name = "name",
            Description = null
        };

        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[1].ErrorMessage == "Description must be not null or empty");
    }
}