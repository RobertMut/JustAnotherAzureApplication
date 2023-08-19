using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Application.Groups.Commands.AddGroup;
using NUnit.Framework;

namespace Application.UnitTests.Groups.Commands.AddGroup;

[ExcludeFromCodeCoverage]
[TestFixture]
public class AddGroupCommandValidatorTests
{
    private AddGroupCommandValidator _validator;

    [SetUp]
    public async Task SetUp()
    {
        _validator = new AddGroupCommandValidator();
    }

    [Test]
    public async Task ValidatorDoesNotThrowException()
    {
        var query = new AddGroupCommand()
        {
            Name = "name",
            Description = "desc"
        };
        
        Assert.DoesNotThrowAsync(async () => await _validator.ValidateAsync(query));
    }
    
    [Test]
    public async Task ValidatorThrowsAtEmptyGroupName()
    {
        var query = new AddGroupCommand()
        {
            Name = "",
            Description = "desc"
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Name must be not null or empty");
    }
    
    [Test]
    public async Task ValidatorThrowsAtEmptyDescription()
    {
        var query = new AddGroupCommand()
        {
            Name = "name",
            Description = ""
        };

        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Description must be not null or empty");
    }
    
    [Test]
    public async Task ValidatorThrowsAtGroupNameIsTooLong()
    {
        var query = new AddGroupCommand()
        {
            Name = new string('a', 512),
            Description = "desc"
        };

        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Group name is too long.");
    }
    
    [Test]
    public async Task ValidatorThrowsAtDescriptionIsTooLong()
    {
        var query = new AddGroupCommand()
        {
            Name = "name",
            Description = new string('a', 4000)
        };

        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Group description is too long.");
    }
}