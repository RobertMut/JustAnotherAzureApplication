using Application.Common.Interfaces.Database;
using Azure.Storage.Blobs.Specialized;
using Functions.UnitTests.Common;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using File = Domain.Entities.File;
using System;
using System.Diagnostics.CodeAnalysis;
using Domain.Common.Helper.Filename;
using Application.Common.Interfaces.Image;
using System.Linq.Expressions;
using Application.Common.Virtuals;
using Functions.UnitTests.Common.Mocking;
using Moq;

namespace Functions.UnitTests;
[ExcludeFromCodeCoverage]
public class MiniaturizeUnitTests
{
    private Miniaturize _miniaturize;
    private IUnitOfWork _unitOfWork;
    private Mock<IImageEditor> _imageEditor;
    private IJAAADbContext _dbContext;
    private Mock<Repository<File>> _fileRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _dbContext = Mock.Of<IJAAADbContext>();
        _fileRepositoryMock = new Mock<Repository<File>>(_dbContext);
        _unitOfWork = new UnitOfWorkMock(_fileRepositoryMock).GetMockedUnitOfWork();
        _imageEditor = new Mock<IImageEditor>();
        _miniaturize = new Miniaturize(_unitOfWork, _imageEditor.Object);
    }

    [Test]
    public async Task Miniaturize()
    {
        var userId = Guid.NewGuid();
        var originalFile = NameHelper.GenerateOriginal(userId.ToString(), "file.png");
        var miniatureFile = NameHelper.GenerateMiniature(userId.ToString(), "30x30", "file.Jpeg");
        _imageEditor.Setup(x => x.Resize(It.IsAny<BlobBaseClient>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(miniatureFile);

        _fileRepositoryMock.Setup(x => x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new File
        {
            Filename = originalFile,
            UserId = userId
        });
        _fileRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<File>(), It.IsAny<CancellationToken>()));
        
        var baseClient = new MockBlobBaseClient(10000, "image/Png", null);

        Assert.DoesNotThrowAsync(async () =>
        {
            await _miniaturize.Run(new MemoryStream(), baseClient, originalFile, NullLogger.Instance);
        });
    }
}