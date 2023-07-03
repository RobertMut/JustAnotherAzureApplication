using Application.Common.Exceptions;
using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Images.Commands.DeleteImage;
using Application.UnitTests.Common.Fakes;
using Azure.Storage.Blobs.Models;
using Domain.Entities;
using MediatR;
using Moq;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UnitTests.Images.Commands.DeleteImage;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteGroupShareCommandTests
{
    private Mock<IBlobManagerService> _service;
    private Mock<IMediator> _mediator;
    private IUnitOfWork _unitOfWork;

    [SetUp]
    public async Task SetUp()
    {
        _unitOfWork = new FakeUnitOfWork();
        _service = new Mock<IBlobManagerService>();
        _mediator = new Mock<IMediator>();

        _service.Setup(x => x.GetBlobsInfoByName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                return new BlobItem[]
                {
                    BlobsModelFactory.BlobItem("original_test.png"),
                    BlobsModelFactory.BlobItem("miniature_300x300_test.Png"),
                    BlobsModelFactory.BlobItem("miniature_200x200_test.Tiff")
                }.AsEnumerable();
            });
        _mediator.Setup(x => x.Send(It.IsAny<DeleteImageCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);
    }


    [Test]
    public async Task DeleteFile()
    {
        _service.Setup(x => x.DeleteBlobAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(HttpStatusCode.Accepted);

        var handler = new DeleteImageCommand.DeleteImageCommandHandler(_service.Object, _unitOfWork);
        var command = new DeleteImageCommand()
        {
            Filename = "test.png",
            DeleteMiniatures = true,
            Size = "any",
            UserId = DbSets.UserId.ToString()
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

        var handler = new DeleteImageCommand.DeleteImageCommandHandler(_service.Object, _unitOfWork);
        var command = new DeleteImageCommand()
        {
            Filename = "test.png",
            UserId = DbSets.UserId.ToString()
        };

        Assert.ThrowsAsync<OperationFailedException>(async () =>
        {
            await handler.Handle(command, CancellationToken.None);
        });
    }
}