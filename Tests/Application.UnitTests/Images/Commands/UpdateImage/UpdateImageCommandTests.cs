using Application.Common.Exceptions;
using Application.Common.Interfaces.Blob;
using Application.Images.Commands.UpdateImage;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UnitTests.Images.Commands.UpdateImage;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UpdateImageCommandTests
{
    private Mock<IBlobManagerService> _service;

    [SetUp]
    public async Task SetUp()
    {
        _service = new Mock<IBlobManagerService>();
    }

    [Test]
    public async Task HandleDoesNotThrow()
    {
        Guid userId = Guid.NewGuid();
        _service.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(HttpStatusCode.OK);
        _service.Setup(x =>
                x.PromoteBlobVersionAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpStatusCode.Created);

        var handler = new UpdateImageCommand.UpdateImageCommandHandler(_service.Object);
        var command = new UpdateImageCommand()
        {
            Filename = "sample.jpeg",
            Height = 1,
            TargetType = Domain.Enums.Image.Format.png,
            Version = 1,
            Width = 1,
            UserId = userId.ToString()
        };

        Assert.DoesNotThrowAsync(async () => await handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task ThrowsOperationFailedWhenPromotingException()
    {
        Guid userId = Guid.NewGuid();
        _service.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(HttpStatusCode.OK);
        _service.Setup(x =>
                x.PromoteBlobVersionAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpStatusCode.InternalServerError);

        var handler = new UpdateImageCommand.UpdateImageCommandHandler(_service.Object);
        var command = new UpdateImageCommand()
        {
            Filename = "sample.jpeg",
            Height = 1,
            TargetType = Domain.Enums.Image.Format.png,
            Version = 1,
            Width = 1,
            UserId = userId.ToString()
        };

        Assert.ThrowsAsync<OperationFailedException>(async () =>
        {
            await handler.Handle(command, CancellationToken.None);
        });
    }

    [Test]
    public async Task ThrowsOperationFailedWhenUpdatingException()
    {
        Guid userId = Guid.NewGuid();
        _service.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(HttpStatusCode.InternalServerError);
        _service.Setup(x =>
                x.PromoteBlobVersionAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpStatusCode.Created);

        var handler = new UpdateImageCommand.UpdateImageCommandHandler(_service.Object);
        var command = new UpdateImageCommand()
        {
            Filename = "sample.jpeg",
            Height = 1,
            TargetType = Domain.Enums.Image.Format.png,
            Version = 1,
            Width = 1,
            UserId = userId.ToString()
        };

        Assert.ThrowsAsync<OperationFailedException>(async () =>
        {
            await handler.Handle(command, CancellationToken.None);
        });
    }
}