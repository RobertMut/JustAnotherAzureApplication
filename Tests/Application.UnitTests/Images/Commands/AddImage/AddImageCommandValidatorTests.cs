using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Application.Images.Commands.AddImage;
using Domain.Enums.Image;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace Application.UnitTests.Images.Commands.AddImage;

[ExcludeFromCodeCoverage]
[TestFixture]
public class AddImageCommandValidatorTests
{
    private AddImageCommandValidator _validator;
    private byte[] _sampleBytes;

    [SetUp]
    public async Task SetUp()
    {
        _sampleBytes = new byte[] { 255, 255, 255, 255, 255 };
        _validator = new AddImageCommandValidator();
    }

    [Test]
    public async Task ValidatorDoesNotThrowException()
    {
        var query = new AddImageCommand
        {
            File = new FormFile(new MemoryStream(_sampleBytes), 0, _sampleBytes.Length, "test.jpg", "test.jpg"),
            Filename = "test.jpg",
            ContentType = "image/jpeg",
            TargetType = Format.png,
            Width = 100,
            Height = 100,
            UserId = Guid.NewGuid().ToString()
        };
        
        Assert.DoesNotThrowAsync(async () => await _validator.ValidateAsync(query));
    }

    [Test]
    public async Task ValidatorThrowsInvalidFile()
    {
        var query = new AddImageCommand
        {
            File = new FormFile(new MemoryStream(Array.Empty<byte>()), 0, 0, "test.jpg", "test.jpg"),
            Filename = "test.jpg",
            ContentType = "image/jpeg",
            TargetType = Format.png,
            Width = 100,
            Height = 100,
            UserId = Guid.NewGuid().ToString()
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Invalid file");
    }
    
    [Test]
    public async Task ValidatorThrowsWidthMustBeGreaterThanZeroWhenWidthIsZero()
    {
        var query = new AddImageCommand
        {
            File = new FormFile(new MemoryStream(_sampleBytes), 0, _sampleBytes.Length, "test.jpg", "test.jpg"),
            Filename = "test.jpg",
            ContentType = "image/jpeg",
            TargetType = Format.png,
            Width = 0,
            Height = 100,
            UserId = Guid.NewGuid().ToString()
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Width must be greater than 0");
    }
    
    [Test]
    public async Task ValidatorThrowsWidthMustBeGreaterThanZeroWhenWidthIsNegative()
    {
        var query = new AddImageCommand
        {
            File = new FormFile(new MemoryStream(_sampleBytes), 0, _sampleBytes.Length, "test.jpg", "test.jpg"),
            Filename = "test.jpg",
            ContentType = "image/jpeg",
            TargetType = Format.png,
            Width = -1,
            Height = 100,
            UserId = Guid.NewGuid().ToString()
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Width must be greater than 0");
    }
    
    [Test]
    public async Task ValidatorThrowsHeightMustBeGreaterThanZeroWhenHeightIsZero()
    {
        var query = new AddImageCommand
        {
            File = new FormFile(new MemoryStream(_sampleBytes), 0, _sampleBytes.Length, "test.jpg", "test.jpg"),
            Filename = "test.jpg",
            ContentType = "image/jpeg",
            TargetType = Format.png,
            Width = 100,
            Height = 0,
            UserId = Guid.NewGuid().ToString()
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Height must be greater than 0");
    }
    
    [Test]
    public async Task ValidatorThrowsHeightMustBeGreaterThanZeroWhenHeightIsNegative()
    {
        var query = new AddImageCommand
        {
            File = new FormFile(new MemoryStream(_sampleBytes), 0, _sampleBytes.Length, "test.jpg", "test.jpg"),
            Filename = "test.jpg",
            ContentType = "image/jpeg",
            TargetType = Format.png,
            Width = 100,
            Height = -1,
            UserId = Guid.NewGuid().ToString()
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Height must be greater than 0");
    }
    
    [Test]
    public async Task ValidatorThrowsOnWrongContentType()
    {
        var query = new AddImageCommand
        {
            File = new FormFile(new MemoryStream(_sampleBytes), 0, _sampleBytes.Length, "test.jpg", "test.jpg"),
            Filename = "test.jpg",
            ContentType = "video/mpeg",
            TargetType = Format.png,
            Width = 100,
            Height = 100,
            UserId = Guid.NewGuid().ToString()
        };
        
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.Errors[0].ErrorMessage == "Invalid content type");
    }
}