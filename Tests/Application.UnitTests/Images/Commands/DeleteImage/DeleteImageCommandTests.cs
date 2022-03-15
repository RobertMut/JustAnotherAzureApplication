using Application.Common.Exceptions;
using Application.Common.Interfaces.Blob;
using Application.Images.Commands.DeleteImage;
using Azure.Storage.Blobs.Models;
using MediatR;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UnitTests.Images.Commands.DeleteImage
{
    public class DeleteImageCommandTests
    {
        private Mock<IBlobManagerService> _service;
        private Mock<IMediator> _mediator;

        [SetUp]
        public async Task SetUp()
        {

            _service = new Mock<IBlobManagerService>();
            _mediator = new Mock<IMediator>();

            _service.Setup(x => x.GetBlobsInfoByName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    return new BlobItem[]
                    {
                        BlobsModelFactory.BlobItem("original-sample.jpg"),
                        BlobsModelFactory.BlobItem("miniature-300x300-sample.Png"),
                        BlobsModelFactory.BlobItem("miniature-200x200-sample.Tiff")
                    }.AsEnumerable();
                });
            _mediator.Setup(x => x.Send(It.IsAny<DeleteImageCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);
        }

        [Test]
        public async Task DeleteFile()
        {
            _service.Setup(x => x.DeleteBlobAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(HttpStatusCode.Accepted);

            var handler = new DeleteImageCommand.DeleteImageCommandHandler(_service.Object);
            var command = new DeleteImageCommand()
            {
                Filename = "sample.jpeg",
            };
            var responseMediator = await _mediator.Object.Send(command, CancellationToken.None);
            var response = await handler.Handle(command, CancellationToken.None);

            Assert.IsInstanceOf(typeof(Unit), response);
            Assert.IsInstanceOf(typeof(Unit), responseMediator);
            Assert.That(responseMediator, Is.Not.Null);
        }

        [Test]
        public async Task DeleteOperationFailedException()
        {
            _service.Setup(x => x.DeleteBlobAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(HttpStatusCode.InternalServerError);

            var handler = new DeleteImageCommand.DeleteImageCommandHandler(_service.Object);
            var command = new DeleteImageCommand()
            {
                Filename = "sample.jpeg",
            };

            Assert.ThrowsAsync<OperationFailedException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });
        }
    }
}
