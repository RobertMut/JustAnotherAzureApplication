using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Common.Models.File;
using Application.Images.Queries.GetFile;
using Azure.Storage.Blobs.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Virtuals;
using Application.UnitTests.Common.Mocking;
using Domain.Common.Helper.Filename;
using Newtonsoft.Json;
using File = Domain.Entities.File;

namespace Application.UnitTests.Images.Queries.GetFile;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetFileQueryTests
{
    private Mock<IBlobManagerService> _service;
    private IUnitOfWork _unitOfWork;
    private Mock<Repository<File>> _fileRepositoryMock;
    private IJAAADbContext _dbContext;

    [SetUp]
    public async Task SetUp()
    {
        _dbContext = Mock.Of<IJAAADbContext>();
        _service = new Mock<IBlobManagerService>();
        _fileRepositoryMock = new Mock<Repository<File>>(_dbContext);
        
        _unitOfWork = new UnitOfWorkMock(_fileRepositoryMock)
            .GetMockedUnitOfWork();
    }

    [Test]
    public async Task GetFile()
    {
        Guid userId = Guid.NewGuid();
        var blob = new Mock<BlobDownloadResult>();
        
        _service.Setup(x => x.DownloadAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(blob.Object);
        _fileRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new File
            {
                Filename = NameHelper.GenerateOriginal(userId.ToString(), NameHelper.GenerateHashedFilename("test")) + ".Png",
                OriginalName = "test.Jpeg",
                UserId = userId
            });

        var handler = new GetFileQueryHandler(_service.Object, _unitOfWork);
        var query = new GetFileQuery()
        {
            Filename = "test.Jpeg",
            UserId = userId.ToString(),
            IsOriginal = true,
            ExpectedExtension = "Png",
            ExpectedMiniatureSize = "100x100"
        };

        Assert.DoesNotThrowAsync(async () =>
        {
            var responseFromHandler = await handler.Handle(query, CancellationToken.None);
            var expected = new FileVm
            {
                File = null
            };
            
            Assert.AreEqual(JsonConvert.SerializeObject(responseFromHandler), JsonConvert.SerializeObject(expected));
        });
    }

    [Test]
    public async Task GetFileThrowsFileNotFound()
    {
        Guid userId = Guid.NewGuid();
        var blob = new Mock<BlobDownloadResult>();
        
        _service.Setup(x => x.DownloadAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(blob.Object);
        _fileRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(File));

        var handler = new GetFileQueryHandler(_service.Object, _unitOfWork);
        var query = new GetFileQuery()
        {
            Filename = "test.Jpeg",
            UserId = userId.ToString(),
            IsOriginal = true,
            ExpectedExtension = "Png",
            ExpectedMiniatureSize = "100x100"
        };

        Assert.ThrowsAsync<FileNotFoundException>(async () => await handler.Handle(query, CancellationToken.None));
    }
}