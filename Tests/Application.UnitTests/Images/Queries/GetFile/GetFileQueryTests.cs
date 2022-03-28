using Application.Common.Interfaces.Blob;
using Application.Common.Models.File;
using Application.Images.Queries.GetFile;
using Azure.Storage.Blobs.Models;
using MediatR;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UnitTests.Images.Queries.GetFile
{
    public class GetFileQueryTests
    {
        private Mock<IBlobManagerService> _service;
        private Mock<IMediator> _mediator;
        private Guid _userId;
        [SetUp]
        public async Task SetUp()
        {
            var blob = new Mock<BlobDownloadResult>();
            _service = new Mock<IBlobManagerService>();
            _mediator = new Mock<IMediator>();
            _userId = Guid.NewGuid();

            _service.Setup(x => x.DownloadAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(blob.Object);
            _mediator.Setup(x => x.Send(It.IsAny<GetFileQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FileVm()
            {
                File = blob.Object
            });
        }

        [Test]
        public async Task GetFile()
        {
            var handler = new GetFileQueryHandler(_service.Object);
            var query = new GetFileQuery()
            {
                Filename = "miniature_50x50_file.png",
                UserId = _userId.ToString()
            };

            Assert.DoesNotThrowAsync(async () =>
            {
                var responseFromHandler = await handler.Handle(query, CancellationToken.None);
                var response = await _mediator.Object.Send(query, CancellationToken.None);

                Assert.IsInstanceOf<FileVm>(responseFromHandler);
                Assert.IsInstanceOf<FileVm>(response);
            });
        }
    }
}
