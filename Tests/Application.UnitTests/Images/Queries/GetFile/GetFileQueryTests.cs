using Application.Common.Interfaces;
using Application.Images.Queries;
using Azure.Storage.Blobs.Models;
using MediatR;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UnitTests.Images.Queries.GetFile
{
    public class GetFileQueryTests
    {
        private Mock<IBlobManagerService> _service;
        private Mock<IMediator> _mediator;

        [SetUp]
        public async Task SetUp()
        {
            var blob = new Mock<BlobDownloadResult>();
            _service = new Mock<IBlobManagerService>();
            _service.Setup(x => x.DownloadAsync(It.IsAny<string>()))
                .ReturnsAsync(blob.Object);
            _mediator = new Mock<IMediator>();
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
                Filename = "file.png"
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
