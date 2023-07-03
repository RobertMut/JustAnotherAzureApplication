using Application.Common.Exceptions;
using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Images.Commands.AddImage;
using Application.UnitTests.Common.Fakes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UnitTests.Images.Commands.AddImage;

[ExcludeFromCodeCoverage]
[TestFixture]
public class AddImageCommandTests
{
    private Mock<IMediator> _mediator;
    private Mock<Stream> _stream;
    private Mock<IBlobManagerService> _service;
    private IUnitOfWork _unitOfWork;
    private Guid _userId;

    [SetUp]
    public async Task SetUp()
    {
        _mediator = new Mock<IMediator>();
        _service = new Mock<IBlobManagerService>();
        _stream = new Mock<Stream>();
        _userId = Guid.NewGuid();
        _unitOfWork = new FakeUnitOfWork();

    }

    [Test]
    public async Task HandleDoesNotThrow()
    {
        _service.Setup(x => x.AddAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<IDictionary<string, string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(HttpStatusCode.Created);
        _mediator.Verify(x => x.Send(It.IsAny<AddImageCommand>(), It.IsAny<CancellationToken>()), Times.Never);

        var handler = new AddImageCommand.AddImageCommandHandler(_service.Object, _unitOfWork);
        var command = new AddImageCommand()
        {
            ContentType = "image/jpeg",
            File = new FormFile(_stream.Object, 0, _stream.Object.Length, "file", "sample.jpg"),
            Filename = "sample.jpg",
            TargetType = Domain.Enums.Image.Format.jpg,
            UserId = _userId.ToString()
        };

        var responseMediator = await _mediator.Object.Send(command, CancellationToken.None);
        var response = await handler.Handle(command, CancellationToken.None);

        Assert.IsInstanceOf(typeof(string), response);
    }

    [Test]
    public async Task ThrowsNullReferenceException()
    {
        _service.Setup(x => x.AddAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<IDictionary<string, string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(HttpStatusCode.InternalServerError);

        var handler = new AddImageCommand.AddImageCommandHandler(_service.Object, _unitOfWork);
        var command = new AddImageCommand()
        {
            ContentType = "image/jpeg",
            File = null,
            Filename = "sample.jpg",
            TargetType = Domain.Enums.Image.Format.png,
            UserId = _userId.ToString()
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
        _service.Setup(x => x.AddAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<IDictionary<string, string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(HttpStatusCode.InternalServerError);
        stream.Setup(x => x.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
            .Callback((byte[] buffer, int offset, int size) =>
                {
                    Array.Copy(new byte[] { }, buffer, 0);
                }
            );

        var handler = new AddImageCommand.AddImageCommandHandler(_service.Object, _unitOfWork);
        var command = new AddImageCommand()
        {
            ContentType = "image/jpeg",
            File = new FormFile(stream.Object, 0, stream.Object.Length, "file", "broken.jpg"),
            Filename = "sample.jpg",
            TargetType = Domain.Enums.Image.Format.png,
            UserId = _userId.ToString()
        };

        Assert.ThrowsAsync<OperationFailedException>(async () =>
        {
            await handler.Handle(command, CancellationToken.None);
        });
    }
}