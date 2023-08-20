using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Application.Images.Commands.UpdateImage;
using Domain.Enums.Image;
using NUnit.Framework;

namespace Application.UnitTests.Images.Commands.UpdateImage;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UpdateImageCommandValidatorTests
{
    private UpdateImageCommandValidator _validator;

    [SetUp]
    public async Task SetUp()
    {
        _validator = new UpdateImageCommandValidator();
    }

    [Test]
    public async Task ValidatorDoesNotThrowException()
    {
        var query = new UpdateImageCommand
        {
            Filename = "file.jpg",
            Width = 100,
            Height = 100,
            TargetType = Format.png,
            Version = 1,
            UserId = Guid.NewGuid().ToString()
        };
        
        Assert.DoesNotThrowAsync(async () => await _validator.ValidateAsync(query));
    }

    [Test]
    public async Task ValidatorThrowsWidthMustBeGreaterThanZeroOnZeroedWidth()
    {
        var query = new UpdateImageCommand
        {
            Filename = "file.jpg",
            Width = 0,
            Height = 100,
            TargetType = Format.png,
            Version = 1,
            UserId = Guid.NewGuid().ToString()
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Width must be greater than 0");
    }
    
    [Test]
    public async Task ValidatorThrowsWidthMustBeGreaterThanZeroOnNegativeWidth()
    {
        var query = new UpdateImageCommand
        {
            Filename = "file.jpg",
            Width = -1,
            Height = 100,
            TargetType = Format.png,
            Version = 1,
            UserId = Guid.NewGuid().ToString()
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Width must be greater than 0");
    }
    
    [Test]
    public async Task ValidatorThrowsHeightMustBeGreaterThanZeroOnZeroedHeight()
    {
        var query = new UpdateImageCommand
        {
            Filename = "file.jpg",
            Width = 100,
            Height = 0,
            TargetType = Format.png,
            Version = 1,
            UserId = Guid.NewGuid().ToString()
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Height must be greater than 0");
    }
    
    [Test]
    public async Task ValidatorThrowsHeightMustBeGreaterThanZeroOnNegativeHeight()
    {
        var query = new UpdateImageCommand
        {
            Filename = "file.jpg",
            Width = 100,
            Height = -1,
            TargetType = Format.png,
            Version = 1,
            UserId = Guid.NewGuid().ToString()
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Height must be greater than 0");
    }
}