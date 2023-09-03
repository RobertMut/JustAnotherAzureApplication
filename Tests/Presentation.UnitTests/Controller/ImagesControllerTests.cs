using System.Diagnostics.CodeAnalysis;
using API.Controllers;
using Application.Common.Interfaces.Identity;
using Application.Common.Models.File;
using Application.Images.Commands.AddImage;
using Application.Images.Commands.DeleteImage;
using Application.Images.Commands.UpdateImage;
using Application.Images.Queries.GetFile;
using Application.Images.Queries.GetUserFiles;
using Azure.Storage.Blobs.Models;
using Domain.Enums.Image;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace Presentation.UnitTests.Controller;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ImagesControllerTests
{
    private Mock<IMediator> _mediatorMock;
    private Mock<ICurrentUserService> _currentUserServiceMock;

    [SetUp]
    public async Task SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _currentUserServiceMock.Setup(x => x.UserId)
            .Returns(Guid.NewGuid().ToString);
    }

    [Test]
    public async Task GetUserFilesTest()
    {
        ImagesController imagesController = new ImagesController(_mediatorMock.Object, _currentUserServiceMock.Object);
        var expectedUserFiles = new UserFilesListVm
        {
            Files = new List<FileDto>()
            {
                new()
                {
                    Filename = "file",
                    IsOwned = true,
                    Permission = Permissions.full,
                    OriginalName = "original"
                }
            }
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetUserFilesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUserFiles);

        Assert.DoesNotThrowAsync(async () =>
        {
            var response = await imagesController.GetUserImages();

            var userFilesListVm = (response as OkObjectResult).Value as UserFilesListVm;
            Assert.IsAssignableFrom(typeof(OkObjectResult), response);
            Assert.That(JsonConvert.SerializeObject(userFilesListVm),
                Is.EqualTo(JsonConvert.SerializeObject(expectedUserFiles)));
        });
    }

    [Test]
    public async Task GetImageTest()
    {
        ImagesController imagesController = new ImagesController(_mediatorMock.Object, _currentUserServiceMock.Object);
        var expectedFile = new FileVm
        {
            File = BlobsModelFactory.BlobDownloadResult(new BinaryData(new byte[] { 255, 255, 255 }),
                BlobsModelFactory.BlobDownloadDetails(BlobType.Block, default, "image/jpeg", default, default, default,
                    default, default, default, default, default, default, default, default, default, default, default,
                    default, default, default, default, default, default, default, default, default, default, default,
                    default, default, default, default))
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetFileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFile);

        Assert.DoesNotThrowAsync(async () =>
        {
            var response = await imagesController.GetImageAsync(new GetFileQuery());

            var userFilesListVm = (response as FileContentResult);
            Assert.IsAssignableFrom(typeof(FileContentResult), response);
            Assert.That(userFilesListVm.FileContents.ToArray(), Is.EqualTo(expectedFile.File.Content.ToArray()));
        });
    }

    [Test]
    public async Task UpdateImageTest()
    {
        ImagesController imagesController = new ImagesController(_mediatorMock.Object, _currentUserServiceMock.Object);

        _mediatorMock.Setup(x => x.Send(It.IsAny<UpdateImageCommand>(), It.IsAny<CancellationToken>()));

        Assert.DoesNotThrowAsync(async () =>
        {
            var response =
                await imagesController.UpdateExistingMiniature(default, default, default, default, default);

            Assert.IsAssignableFrom(typeof(OkResult), response);
        });
    }

    [Test]
    public async Task DeleteImageTest()
    {
        ImagesController imagesController = new ImagesController(_mediatorMock.Object, _currentUserServiceMock.Object);

        _mediatorMock.Setup(x => x.Send(It.IsAny<DeleteImageCommand>(), It.IsAny<CancellationToken>()));

        Assert.DoesNotThrowAsync(async () =>
        {
            var response =
                await imagesController.DeleteImageAsync(default, default, default);

            Assert.IsAssignableFrom(typeof(OkResult), response);
        });
    }

    [Test]
    public async Task PostImageTest()
    {
        ImagesController imagesController = new ImagesController(_mediatorMock.Object, _currentUserServiceMock.Object);

        string expectedFilename = "filename";
        using var stream = new MemoryStream(new byte[] { 255, 255, 255, 255, 255 }, true);
        FormFile formFile = new FormFile(stream, stream.Position, stream.Capacity, "file", "filename")
        {
            Headers = new HeaderDictionary()
        };

        formFile.ContentType = "image/jpeg";

        _mediatorMock.Setup(x => x.Send(It.IsAny<AddImageCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFilename);

        Assert.DoesNotThrowAsync(async () =>
        {
            var response =
                await imagesController.PostImageAsync(formFile, Format.bmp, 100, 50);

            var filename = (response as OkObjectResult).Value as string;
            Assert.IsAssignableFrom(typeof(OkObjectResult), response);
            Assert.That(filename, Is.EqualTo(expectedFilename));
        });
    }
}