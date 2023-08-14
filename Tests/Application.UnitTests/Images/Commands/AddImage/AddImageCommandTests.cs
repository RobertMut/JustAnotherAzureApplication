using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Virtuals;
using Application.Images.Commands.AddImage;
using Application.UnitTests.Common.Mocking;
using Domain.Common.Helper.Filename;
using File = Domain.Entities.File;

namespace Application.UnitTests.Images.Commands.AddImage;

[ExcludeFromCodeCoverage]
[TestFixture]
public class AddImageCommandTests
{
    private Mock<Stream> _stream;
    private Mock<IBlobManagerService> _service;
    private IUnitOfWork _unitOfWork;
    private Mock<Repository<File>> _fileRepositoryMock;
    private IJAAADbContext _dbContext;

    [SetUp]
    public async Task SetUp()
    {
        _dbContext = Mock.Of<IJAAADbContext>();
        _service = new Mock<IBlobManagerService>();
        _stream = new Mock<Stream>();
        _fileRepositoryMock = new Mock<Repository<File>>(_dbContext);
        _unitOfWork = new UnitOfWorkMock(_fileRepositoryMock)
            .GetMockedUnitOfWork();

    }

    [Test]
    public async Task HandleDoesNotThrow()
    {
        Guid userId = Guid.NewGuid();
        var file = new File
        {
            Filename = NameHelper.GenerateMiniature(userId.ToString(), "300x300",
                NameHelper.GenerateHashedFilename("image.Png")),
            OriginalName = "image.Png",
            UserId = userId,
        };
        _fileRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(file);
        
        _service.Setup(x => x.AddAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<IDictionary<string, string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(HttpStatusCode.Created);

        var handler = new AddImageCommand.AddImageCommandHandler(_service.Object, _unitOfWork);
        var command = new AddImageCommand()
        {
            ContentType = "image/jpeg",
            File = new FormFile(_stream.Object, 0, _stream.Object.Length, "file", "sample.jpg"),
            Filename = file.Filename,
            TargetType = Domain.Enums.Image.Format.jpg,
            UserId = userId.ToString()
        };
        
        var response = await handler.Handle(command, CancellationToken.None);
        
        _fileRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(response == file.Filename);
    }

    [Test]
    public async Task HandleDoesNotThrowAndCreatesNewImageEntry()
    {
        Guid userId = Guid.NewGuid();
        var file = new File
        {
            Filename = NameHelper.GenerateMiniature(userId.ToString(), "300x300",
                NameHelper.GenerateHashedFilename("image.Png")),
            OriginalName = "image.Png",
            UserId = userId,
        };
        _fileRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(File));
        _fileRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<File>(), It.IsAny<CancellationToken>()));
        
        _service.Setup(x => x.AddAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<IDictionary<string, string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(HttpStatusCode.Created);

        var handler = new AddImageCommand.AddImageCommandHandler(_service.Object, _unitOfWork);
        var command = new AddImageCommand()
        {
            ContentType = "image/jpeg",
            File = new FormFile(_stream.Object, 0, _stream.Object.Length, "file", "sample.jpg"),
            Filename = file.Filename,
            TargetType = Domain.Enums.Image.Format.jpg,
            UserId = userId.ToString()
        };
        
        var response = await handler.Handle(command, CancellationToken.None);
        
        _fileRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<File>(), It.IsAny<CancellationToken>()), Times.Once);
        _fileRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        
        Assert.True(response == file.Filename);
    }
}