using Application.Common.Exceptions;
using Application.Common.Interfaces.Blob;
using Application.Images.Commands.AddImage;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UnitTests.Images.Commands.AddImage
{
    public class AddImageCommandTests
    {
        private Mock<IMediator> _mediator;
        private Mock<Stream> _stream;
        private Mock<IBlobManagerService> _service;

        [SetUp]
        public async Task SetUp()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<AddImageCommand>(), It.IsAny<CancellationToken>())).Returns(Unit.Task);
            _stream = new Mock<Stream>();
            _stream.Setup(x => x.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Callback((byte[] buffer, int offset, int size) =>
                {
                    Array.Copy(new byte[] { 00, 00, 00, 00, 00, 00 }, buffer, 6);
                }
                );
            _service = new Mock<IBlobManagerService>();

        }
        [Test]
        public async Task HandleDoesNotThrow()
        {
            _service.Setup(x => x.AddAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(),
                 It.IsAny<IDictionary<string, string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(201);

            var handler = new AddImageCommand.AddImageCommandHandler(_service.Object);
            var command = new AddImageCommand()
            {
                ContentType = "image/jpeg",
                File = new FormFile(_stream.Object, 0, _stream.Object.Length, "file", "sample.jpg"),
                FileName = "sample.jpg",
                TargetType = "png"
            };
            var responseMediator = await _mediator.Object.Send(command, CancellationToken.None);

            _mediator.Verify(x => x.Send(It.IsAny<AddImageCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            var response = await handler.Handle(command, CancellationToken.None);

            Assert.IsInstanceOf(typeof(Unit), response);
            Assert.IsInstanceOf(typeof(Unit), responseMediator);
            Assert.That(responseMediator, Is.Not.Null);
        }
        [Test]
        public async Task ThrowsNullReferenceException()
        {
            _service.Setup(x => x.AddAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<IDictionary<string, string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(500);

            var handler = new AddImageCommand.AddImageCommandHandler(_service.Object);
            var command = new AddImageCommand()
            {
                ContentType = "image/jpeg",
                File = null,
                FileName = "sample.jpg",
                TargetType = "png"
            };

            Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });

        }
        [Test]
        public async Task ThrowsOperationFailedException()
        {
            var stream = new Mock<Stream>();
            stream.Setup(x => x.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Callback((byte[] buffer, int offset, int size) =>
                {
                    Array.Copy(new byte[] { }, buffer, 0);
                }
                );
            _service.Setup(x => x.AddAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<IDictionary<string, string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(500);
            var handler = new AddImageCommand.AddImageCommandHandler(_service.Object);
            var command = new AddImageCommand()
            {
                ContentType = "image/jpeg",
                File = new FormFile(stream.Object, 0, stream.Object.Length, "file", "broken.jpg"),
                FileName = "sample.jpg",
                TargetType = "png"
            };

            Assert.ThrowsAsync<OperationFailedException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });

        }


    }
}
