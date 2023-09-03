using System.Diagnostics.CodeAnalysis;
using API.Controllers;
using Application.Common.Interfaces.Identity;
using Application.Groups.Commands.AddGroup;
using Application.Groups.Commands.DeleteGroup;
using Application.Groups.Queries.GetGroups;
using Application.GroupShares.Commands.AddGroupShare;
using Application.GroupShares.Queries.GetSharesByGroup;
using Application.UserShares.Commands.AddUserShare;
using Application.UserShares.Commands.DeleteUserShare;
using Application.UserShares.Queries.GetSharesByUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Presentation.UnitTests.Controller;

[ExcludeFromCodeCoverage]
[TestFixture]
public class SharesControllerTests
{
    private Mock<IMediator> _mediatorMock;

    [SetUp]
    public async Task SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
    }

    [Test]
    public async Task GetUserSharesTest()
    {
        var expectedShares = new UserSharesListVm
        {
            Shares = new List<UserSharesDto>()
            {
                new()
                {
                    Filename = "file",
                    Permissions = "readwrite"
                }
            }
        };
        
        SharesController sharesController = new SharesController(_mediatorMock.Object);

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetSharesByUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedShares);

        Assert.DoesNotThrowAsync(async () =>
        {
            var response =
                await sharesController.GetUserShares(Guid.NewGuid());

            Assert.IsAssignableFrom(typeof(OkObjectResult), response);
            var userSharesListVm = (response as OkObjectResult).Value as UserSharesListVm;
            Assert.IsAssignableFrom(typeof(OkObjectResult), response);
            Assert.That(JsonConvert.SerializeObject(userSharesListVm),
                Is.EqualTo(JsonConvert.SerializeObject(expectedShares)));
        });
    }
    
    [Test]
    public async Task GetGroupSharesTest()
    {
        var expectedShares = new GroupSharesListVm
        {
            Shares = new List<GroupSharesDto>()
            {
                new()
                {
                    Filename = "file",
                    Permissions = "readwrite"
                }
            }
        };
        
        SharesController sharesController = new SharesController(_mediatorMock.Object);

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetSharesByGroupQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedShares);

        Assert.DoesNotThrowAsync(async () =>
        {
            var response =
                await sharesController.GetGroupShares(Guid.NewGuid().ToString());

            Assert.IsAssignableFrom(typeof(OkObjectResult), response);
            var groupSharesVm = (response as OkObjectResult).Value as GroupSharesListVm;
            Assert.IsAssignableFrom(typeof(OkObjectResult), response);
            Assert.That(JsonConvert.SerializeObject(groupSharesVm),
                Is.EqualTo(JsonConvert.SerializeObject(expectedShares)));
        });
    }
    
    [Test]
    public async Task AddGroupSharesTest()
    {
        string expectedValue = "value";
        
        SharesController sharesController = new SharesController(_mediatorMock.Object);

        _mediatorMock.Setup(x => x.Send(It.IsAny<AddGroupShareCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedValue);

        Assert.DoesNotThrowAsync(async () =>
        {
            var response =
                await sharesController.AddGroupShare(new AddGroupShareCommand());

            Assert.IsAssignableFrom(typeof(OkObjectResult), response);
            string value = (response as OkObjectResult).Value as string;
            Assert.IsAssignableFrom(typeof(OkObjectResult), response);
            Assert.That(value, Is.EqualTo(expectedValue));
        });
    }
    
    [Test]
    public async Task AddUserSharesTest()
    {
        string expectedValue = "value";
        
        SharesController sharesController = new SharesController(_mediatorMock.Object);

        _mediatorMock.Setup(x => x.Send(It.IsAny<AddUserShareCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedValue);

        Assert.DoesNotThrowAsync(async () =>
        {
            var response =
                await sharesController.AddUserShare(new AddUserShareCommand());

            Assert.IsAssignableFrom(typeof(OkObjectResult), response);
            string value = (response as OkObjectResult).Value as string;
            Assert.IsAssignableFrom(typeof(OkObjectResult), response);
            Assert.That(value, Is.EqualTo(expectedValue));
        });
    }
    
    [Test]
    public async Task DeleteUserSharesTest()
    {
        SharesController sharesController = new SharesController(_mediatorMock.Object);

        _mediatorMock.Setup(x => x.Send(It.IsAny<DeleteUserShareCommand>(), It.IsAny<CancellationToken>()));

        Assert.DoesNotThrowAsync(async () =>
        {
            var response =
                await sharesController.DeleteUserShare(default, default);

            Assert.IsAssignableFrom(typeof(OkResult), response);
        });
    }
    
    [Test]
    public async Task DeleteGroupSharesTest()
    {
        SharesController sharesController = new SharesController(_mediatorMock.Object);

        _mediatorMock.Setup(x => x.Send(It.IsAny<DeleteGroupCommand>(), It.IsAny<CancellationToken>()));

        Assert.DoesNotThrowAsync(async () =>
        {
            var response =
                await sharesController.DeleteGroupShare(default, default);

            Assert.IsAssignableFrom(typeof(OkResult), response);
        });
    }
}