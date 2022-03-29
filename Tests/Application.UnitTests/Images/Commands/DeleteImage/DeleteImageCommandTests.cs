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
using System.Linq.Expressions;
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
        private Mock<IUnitOfWork> _unitOfWork;
        private Guid _userId;
        [SetUp]
        public async Task SetUp()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _service = new Mock<IBlobManagerService>();
            _mediator = new Mock<IMediator>();
            _userId = Guid.NewGuid();

            _unitOfWork.Setup(x => x.FileRepository.GetObjectBy(It.IsAny<Expression<Func<File, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync((string name, CancellationToken cancellationToken) =>
            {
                return new File()
                {
                    Filename = name,
                    UserId = _userId
                };
            });
            _unitOfWork.Setup(x => x.FileRepository.Delete(It.IsAny<File>()));
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

            var handler = new DeleteImageCommand.DeleteImageCommandHandler(_service.Object, _unitOfWork.Object);
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

            var handler = new DeleteImageCommand.DeleteImageCommandHandler(_service.Object, _unitOfWork.Object);
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
