using Application.Common.Exceptions;
using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Images.Commands.DeleteImage;
using Azure.Storage.Blobs.Models;
using Domain.Entities;
using MediatR;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UnitTests.Images.Commands.DeleteImage
{
    [TestFixture]
    public class DeleteImageCommandTests
    {
        private Mock<IBlobManagerService> _service;
        private Mock<IMediator> _mediator;
        private Mock<IRepository<File>> _fileRepository;
        private Guid _userId;
        [SetUp]
        public async Task SetUp()
        {
            _fileRepository = new Mock<IRepository<File>>();
            _service = new Mock<IBlobManagerService>();
            _mediator = new Mock<IMediator>();
            _userId = Guid.NewGuid();

            _fileRepository.Setup(x => x.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((string name, CancellationToken cancellationToken) =>
            {
                return new File()
                {
                    Filename = name,
                    UserId = _userId
                };
            });
            _fileRepository.Setup(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()));
            _service.Setup(x => x.GetBlobsInfoByName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    return new BlobItem[]
                    {
                        BlobsModelFactory.BlobItem("original_sample.jpg"),
                        BlobsModelFactory.BlobItem("miniature_300x300_sample.Png"),
                        BlobsModelFactory.BlobItem("miniature_200x200_sample.Tiff")
                    }.AsEnumerable();
                });
            _mediator.Setup(x => x.Send(It.IsAny<DeleteImageCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);
        }


        [Test]
        public async Task DeleteFile()
        {
            _service.Setup(x => x.DeleteBlobAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(HttpStatusCode.Accepted);

            var handler = new DeleteImageCommand.DeleteImageCommandHandler(_service.Object, _fileRepository.Object);
            var command = new DeleteImageCommand()
            {
                Filename = "sample.jpeg",
                UserId = _userId.ToString()
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

            var handler = new DeleteImageCommand.DeleteImageCommandHandler(_service.Object, _fileRepository.Object);
            var command = new DeleteImageCommand()
            {
                Filename = "sample.jpeg",
                UserId = _userId.ToString()
            };

            Assert.ThrowsAsync<OperationFailedException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });
        }
    }
}
