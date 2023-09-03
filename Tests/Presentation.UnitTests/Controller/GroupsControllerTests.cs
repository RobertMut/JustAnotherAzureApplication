using System.Diagnostics.CodeAnalysis;
using API.Controllers;
using Application.Groups.Commands.AddGroup;
using Application.Groups.Commands.DeleteGroup;
using Application.Groups.Queries.GetGroups;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace Presentation.UnitTests.Controller;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GroupsControllerTests
{
    private Mock<IMediator> _mediatorMock;

    [SetUp]
    public async Task SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
    }

    [Test]
    public async Task GetGroupsTest()
    {
        GroupsController groupsController = new GroupsController(_mediatorMock.Object);
        var expectedGroups = new List<GroupDto>()
        {
            new()
            {
                Description = "desc",
                Id = "id",
                Name = "name"
            }
        };
        
        _mediatorMock.Setup(x => x.Send(It.IsAny<GetGroupsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GroupListVm
            {
                Groups = expectedGroups
            });
        
        Assert.DoesNotThrowAsync(async () =>
        {
            var response = await groupsController.GetGroups();

            var groupListVm = (response as OkObjectResult).Value as GroupListVm;
            Assert.IsAssignableFrom(typeof(OkObjectResult), response);
            Assert.That(JsonConvert.SerializeObject(groupListVm), Is.EqualTo(JsonConvert.SerializeObject(new GroupListVm
            {
                Groups = expectedGroups
            })));
        });
    }
    
    [Test]
    public async Task AddGroupsTest()
    {
        GroupsController groupsController = new GroupsController(_mediatorMock.Object);
        string expected = Guid.NewGuid().ToString();
        
        _mediatorMock.Setup(x => x.Send(It.IsAny<AddGroupCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        
        Assert.DoesNotThrowAsync(async () =>
        {
            var response = await groupsController.AddGroup(new AddGroupCommand());

            var guid = (response as OkObjectResult).Value as string;
            Assert.IsAssignableFrom(typeof(OkObjectResult), response);
            Assert.That(guid, Is.EqualTo(expected));
        });
    }
    
    [Test]
    public async Task DeleteGroupsTest()
    {
        GroupsController groupsController = new GroupsController(_mediatorMock.Object);

        _mediatorMock.Setup(x => x.Send(It.IsAny<DeleteGroupCommand>(), It.IsAny<CancellationToken>()));
        Assert.DoesNotThrowAsync(async () =>
        {
            var response = await groupsController.DeleteGroup(new DeleteGroupCommand());

            Assert.IsAssignableFrom(typeof(OkResult), response);
        });
    }
}