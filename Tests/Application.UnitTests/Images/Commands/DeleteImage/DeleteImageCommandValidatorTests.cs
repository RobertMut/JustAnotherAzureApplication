using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Application.Images.Commands.DeleteImage;
using NUnit.Framework;

namespace Application.UnitTests.Images.Commands.DeleteImage;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteImageCommandValidatorTests
{
    private DeleteImageCommandValidator _validator;

    [SetUp]
    public async Task SetUp()
    {
        _validator = new DeleteImageCommandValidator();
    }

    [Test]
    public async Task ValidatorDoesNotThrowException()
    {
        var query = new DeleteImageCommand
        {
            Filename = "file.png",
            DeleteMiniatures = true,
            Size = "100x100",
            UserId = Guid.NewGuid().ToString()
        };
        
        Assert.DoesNotThrowAsync(async () => await _validator.ValidateAsync(query));
    }

    [Test]
    public async Task ValidatorThrowsFilenameMustNotBeEmpty()
    {
        var query = new DeleteImageCommand
        {
            Filename = "",
            DeleteMiniatures = true,
            Size = "100x100",
            UserId = Guid.NewGuid().ToString()
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Filename must be not empty");
    }
    
    [Test]
    public async Task ValidatorThrowsInvalidDimensionFormat()
    {
        var query = new DeleteImageCommand
        {
            Filename = "file.png",
            DeleteMiniatures = true,
            Size = "invaliddimension",
            UserId = Guid.NewGuid().ToString()
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Invalid dimension format");
    }
    
    [Test]
    public async Task ValidatorThrowsDimensionsMustBeNumericAndGreaterThanZeroOnNegativeValues()
    {
        var query = new DeleteImageCommand
        {
            Filename = "file.png",
            DeleteMiniatures = true,
            Size = "-1x-1",
            UserId = Guid.NewGuid().ToString()
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[1].ErrorMessage == "Dimensions must be numeric and greater than 0");
    }
    
    [Test]
    public async Task ValidatorThrowsDimensionsMustBeNumericAndGreaterThanZeroOnLetters()
    {
        var query = new DeleteImageCommand
        {
            Filename = "file.png",
            DeleteMiniatures = true,
            Size = "axb",
            UserId = Guid.NewGuid().ToString()
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[1].ErrorMessage == "Dimensions must be numeric and greater than 0");
    }
}