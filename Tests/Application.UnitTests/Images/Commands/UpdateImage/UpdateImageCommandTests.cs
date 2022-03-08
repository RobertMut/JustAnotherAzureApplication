using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Images.Commands.UpdateImage;
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
    public class UpdateImageCommandTests
    {
        private Mock<IMediator> _mediator;
        private Mock<IBlobManagerService> _service;

        [SetUp]
        public async Task SetUp()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<UpdateImageCommand>(), It.IsAny<CancellationToken>())).Returns(Unit.Task);
            _service = new Mock<IBlobManagerService>();

        }
        [Test]
        public async Task HandleDoesNotThrow()
        {
            _service.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(200);
            _service.Setup(x => x.PromoteBlobVersionAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(201);
            var handler = new UpdateImageCommand.UpdateImageCommandHandler(_service.Object);
            var command = new UpdateImageCommand()
            {
                Filename = "sample.jpeg",
                Height = 1,
                TargetType = "png",
                Version = 1,
                Width = 1,
            };
            var responseMediator = await _mediator.Object.Send(command, CancellationToken.None);
            _mediator.Verify(x => x.Send(It.IsAny<UpdateImageCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            var response = await handler.Handle(command, CancellationToken.None);
            Assert.IsInstanceOf(typeof(Unit), response);
            Assert.IsInstanceOf(typeof(Unit), responseMediator);
            Assert.That(responseMediator, Is.Not.Null);
        }
        [Test]
        public async Task ThrowsOperationFailedWhenPromotingException()
        {
            _service.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(200);
            _service.Setup(x => x.PromoteBlobVersionAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(500);
            var handler = new UpdateImageCommand.UpdateImageCommandHandler(_service.Object);
            var command = new UpdateImageCommand()
            {
                Filename = "sample.jpeg",
                Height = 1,
                TargetType = "png",
                Version = 1,
                Width = 1,
            };
            Assert.ThrowsAsync<OperationFailedException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });

        }
        [Test]
        public async Task ThrowsOperationFailedWhenUpdatingException()
        {
            _service.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(500);
            _service.Setup(x => x.PromoteBlobVersionAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(201);
            var handler = new UpdateImageCommand.UpdateImageCommandHandler(_service.Object);
            var command = new UpdateImageCommand()
            {
                Filename = "sample.jpeg",
                Height = 1,
                TargetType = "png",
                Version = 1,
                Width = 1,
            };
            Assert.ThrowsAsync<OperationFailedException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });

        }


    }
}
