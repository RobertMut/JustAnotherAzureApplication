using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Application.Images.Queries.GetFile;
using NUnit.Framework;

namespace Application.UnitTests.Images.Queries.GetFile;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetFileQueryValidatorTests
{
    private GetFileQueryValidator _validator;

    [SetUp]
    public async Task SetUp()
    {
        _validator = new GetFileQueryValidator();
    }

    [Test]
    public async Task ValidatorDoesNotThrowException()
    {
        var query = new GetFileQuery
        {
            Filename = "test.jpg",
            Id = 1,
            UserId = Guid.NewGuid().ToString(),
            IsOriginal = true,
            ExpectedExtension = ".png",
            ExpectedMiniatureSize = "100x100"
        };
        
        Assert.DoesNotThrowAsync(async () => await _validator.ValidateAsync(query));
    }

    [Test]
    public async Task ValidatorThrowsInvalidFilename()
    {
        var query = new GetFileQuery
        {
            Filename = "blahblah",
            Id = 1,
            UserId = Guid.NewGuid().ToString(),
            IsOriginal = true,
            ExpectedExtension = "png",
            ExpectedMiniatureSize = "100x100"
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Invalid filename");
    }
    
    [Test]
    public async Task ValidatorThrowsInvalidExtensionFormat()
    {
        var query = new GetFileQuery
        {
            Filename = "test.jpg",
            Id = 1,
            UserId = Guid.NewGuid().ToString(),
            IsOriginal = true,
            ExpectedExtension = ".",
            ExpectedMiniatureSize = "100x100"
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Invalid extension format");
    }
    
    [Test]
    public async Task ValidatorThrowsInvalidDimensionFormat()
    {
        var query = new GetFileQuery
        {
            Filename = "test.jpg",
            Id = 1,
            UserId = Guid.NewGuid().ToString(),
            IsOriginal = true,
            ExpectedExtension = "png",
            ExpectedMiniatureSize = "blahblah"
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Invalid dimension format");
    }
    
    [Test]
    public async Task ValidatorThrowsDimensionsMustBeNumericAndGreaterThanZeroOnNegativeValues()
    {
        var query = new GetFileQuery
        {
            Filename = "test.jpg",
            Id = 1,
            UserId = Guid.NewGuid().ToString(),
            IsOriginal = true,
            ExpectedExtension = "png",
            ExpectedMiniatureSize = "-1x-1"
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[1].ErrorMessage == "Dimensions must be numeric and greater than 0");
    }
    
    [Test]
    public async Task ValidatorThrowsDimensionsMustBeNumericAndGreaterThanZeroOnLetters()
    {
        var query = new GetFileQuery
        {
            Filename = "test.jpg",
            Id = 1,
            UserId = Guid.NewGuid().ToString(),
            IsOriginal = true,
            ExpectedExtension = "png",
            ExpectedMiniatureSize = "axb"
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[1].ErrorMessage == "Dimensions must be numeric and greater than 0");
    }
}