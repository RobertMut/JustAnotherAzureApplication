using Application.Common.Interfaces.Database;
using Azure.Storage.Blobs.Specialized;
using Domain.Constants.Image;
using Functions.UnitTests.Common;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using File = Domain.Entities.File;
using System;
using Domain.Common.Helper.Filename;
using Application.Common.Interfaces.Image;

namespace Functions.UnitTests
{
    public class MiniaturizeUnitTests
    {
        private Miniaturize _miniaturize;
        private Mock<IRepository<File>> _fileRepository;
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
            _fileRepository = new Mock<IRepository<File>>();
            _imageEditor = new Mock<IImageEditor>();

            _imageEditor.Setup(x => x.Resize(It.IsAny<BlobBaseClient>(), It.IsAny<Stream>(), It.IsAny<string>())).ReturnsAsync(_miniatureFile);
            _fileRepository.Setup(x => x.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((string name, CancellationToken cancellationToken) =>
            {
                return new File
                {
                    Filename = name,
                    UserId = _userId
                };
            });
            _fileRepository.Setup(x => x.AddAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()));

            _miniaturize = new Miniaturize(_fileRepository.Object, _imageEditor.Object);
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
}