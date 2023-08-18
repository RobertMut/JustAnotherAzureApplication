using Application.Common.Interfaces.Database;
using Azure.Storage.Blobs.Specialized;
using Functions.UnitTests.Common;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
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

namespace Functions.UnitTests;
[ExcludeFromCodeCoverage]
public class MiniaturizeUnitTests
{
    private Miniaturize _miniaturize;
    private Mock<IUnitOfWork> _unitOfWork;
    private Mock<IImageEditor> _imageEditor;
    private Guid _userId;
    private string _originalFile;
    private string _miniatureFile;

    [SetUp]
    public void Setup()
    {
        _userId = Guid.NewGuid();
        _originalFile = NameHelper.GenerateOriginal(_userId.ToString(), "file.png");
        _miniatureFile = NameHelper.GenerateMiniature(_userId.ToString(), "30x30", "file.Jpeg");
        _unitOfWork = new Mock<IUnitOfWork>();
        _imageEditor = new Mock<IImageEditor>();

        _imageEditor.Setup(x => x.Resize(It.IsAny<BlobBaseClient>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_miniatureFile);
        _unitOfWork.Setup(x => x.FileRepository.GetObjectBy(It.IsAny<Expression<Func<File, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync((Expression<Func<File, bool>> predicate) =>
        {
            return new File
            {
                Filename = _originalFile,
                UserId = _userId
            };
        });
        _unitOfWork.Setup(x => x.FileRepository.InsertAsync(It.IsAny<File>(), It.IsAny<CancellationToken>()));

        _miniaturize = new Miniaturize(_unitOfWork.Object, _imageEditor.Object);
    }

    [Test]
    public async Task Miniaturize()
    {
        var baseClient = new MockBlobBaseClient(10000, "image/Png", null);

        Assert.DoesNotThrowAsync(async () =>
        {
            await _miniaturize.Run(new MemoryStream(), baseClient, _originalFile, NullLogger.Instance);
        });
    }
}