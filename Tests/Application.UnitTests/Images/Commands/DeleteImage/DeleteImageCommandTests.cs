using System;
using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Images.Commands.DeleteImage;
using Azure.Storage.Blobs.Models;
using MediatR;
using Moq;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Virtuals;
using Application.UnitTests.Common.Mocking;
using File = Domain.Entities.File;
using Range = Moq.Range;

namespace Application.UnitTests.Images.Commands.DeleteImage;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteGroupShareCommandTests
{
    private Mock<IBlobManagerService> _service;
    private IUnitOfWork _unitOfWork;
    private Mock<Repository<File>> _fileRepositoryMock;
    private IJAAADbContext _dbContext;

    [SetUp]
    public async Task SetUp()
    {
        _dbContext = Mock.Of<IJAAADbContext>();
        _fileRepositoryMock = new Mock<Repository<File>>(_dbContext);
        _unitOfWork = new UnitOfWorkMock(_fileRepositoryMock)
            .GetMockedUnitOfWork();
        _service = new Mock<IBlobManagerService>();

        _service.Setup(x => x.GetBlobsInfoByName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                return new BlobItem[]
                {
                    BlobsModelFactory.BlobItem("original_test.png"),
                    BlobsModelFactory.BlobItem("miniature_300x300_test.Png"),
                    BlobsModelFactory.BlobItem("miniature_200x200_test.Tiff")
                }.AsEnumerable();
            });
    }


    [Test]
    public async Task DeleteFile()
    {
        Guid userId = Guid.NewGuid();
        File file = new File
        {
            Filename = "miniature_300x300_test.Png",
            OriginalName = "original_test.png",
            UserId = userId,
        };

        _fileRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(file);
        _fileRepositoryMock.Setup(x => x.Delete(It.IsAny<File?>()));
        _service.Setup(x => x.DeleteBlobAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(HttpStatusCode.Accepted);

        var handler = new DeleteImageCommand.DeleteImageCommandHandler(_service.Object, _unitOfWork);
        var command = new DeleteImageCommand()
        {
            Filename = "original_test.png",
            DeleteMiniatures = true,
            Size = "any",
            UserId = userId.ToString()
        };
        var response = await handler.Handle(command, CancellationToken.None);

        Assert.IsInstanceOf(typeof(Unit), response);
        
        _fileRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()), Times.Between(3, 4, Range.Inclusive));
        _fileRepositoryMock.Verify(x => x.Delete(It.IsAny<File?>()), Times.Between(3,4, Range.Inclusive));
        _service.Verify(x => x.DeleteBlobAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Between(3, 4, Range.Inclusive));
    }

    [Test]
    public async Task DeleteFileThrowsFileNotFound()
    {
        Guid userId = Guid.NewGuid();

        _fileRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(File));
        _fileRepositoryMock.Setup(x => x.Delete(It.IsAny<File?>()));
        _service.Setup(x => x.DeleteBlobAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(HttpStatusCode.Accepted);

        var handler = new DeleteImageCommand.DeleteImageCommandHandler(_service.Object, _unitOfWork);
        var command = new DeleteImageCommand()
        {
            Filename = "original_test.png",
            DeleteMiniatures = true,
            Size = "any",
            UserId = userId.ToString()
        };
        Assert.ThrowsAsync<FileNotFoundException>(async () =>
        {
             await handler.Handle(command, CancellationToken.None);
        });
        
        _fileRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _fileRepositoryMock.Verify(x => x.Delete(It.IsAny<File?>()), Times.Never);
        _service.Verify(x => x.DeleteBlobAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}