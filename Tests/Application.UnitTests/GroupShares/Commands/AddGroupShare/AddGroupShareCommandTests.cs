using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq.Expressions;
using Application.Common.Interfaces.Database;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Virtuals;
using Application.GroupShares.Commands.AddGroupShare;
using Application.UnitTests.Common.Mocking;
using Domain.Enums.Image;
using Domain.Common.Helper.Filename;
using Domain.Entities;
using Moq;
using File = Domain.Entities.File;

namespace Application.UnitTests.GroupShares.Commands.AddGroupShare;

[ExcludeFromCodeCoverage]
[TestFixture]
public class AddGroupShareCommandTests
{
    private IUnitOfWork _unitOfWork;
    private AddGroupShareCommand.AddGroupShareCommandHandler _commandHandler;
    private Mock<Repository<GroupUser>>? _groupUserRepositoryMock;
    private Mock<Repository<File>>? _fileRepositoryMock;
    private Mock<Repository<GroupShare>> _groupShareRepositoryMock;
    private IJAAADbContext _dbContext;
    [SetUp]
    public async Task SetUp()
    {
        _dbContext = Mock.Of<IJAAADbContext>();
        _groupUserRepositoryMock = new Mock<Repository<GroupUser>>(_dbContext);
        _fileRepositoryMock = new Mock<Repository<File>>(_dbContext);
        _groupShareRepositoryMock = new Mock<Repository<GroupShare>>(_dbContext);

        _unitOfWork = new UnitOfWorkMock(_fileRepositoryMock, default, _groupShareRepositoryMock, default,
            default, _groupUserRepositoryMock, default).GetMockedUnitOfWork();
        _commandHandler = new AddGroupShareCommand.AddGroupShareCommandHandler(_unitOfWork);
    }

    [Test]
    public async Task HandleDoesNotThrow()
    {
        var groupUser = new GroupUser()
        {
            GroupId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        var file = new File
        {
            Filename = NameHelper.GenerateMiniature(groupUser.UserId.ToString(), "300x300",
                NameHelper.GenerateHashedFilename("notshared.Png")),
            OriginalName = "notshared.Png",
            UserId = groupUser.UserId,
        };
        _groupUserRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<GroupUser?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(groupUser);
        _fileRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(file);
        _groupShareRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<GroupShare?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GroupShare
            {
                GroupId = groupUser.GroupId,
            });
        
        Assert.DoesNotThrowAsync(async () =>
        {
            var groupId = await _commandHandler.Handle(new AddGroupShareCommand
            {
                Filename = NameHelper.GenerateMiniature(groupUser.UserId.ToString(), "300x300", NameHelper.GenerateHashedFilename("notshared.Png")),
                GroupId = groupUser.GroupId.ToString(),
                PermissionId = Permissions.readwrite,
                UserId = groupUser.UserId.ToString()
            }, CancellationToken.None);

            Assert.True(groupId == groupUser.GroupId.ToString());
        });
    }

    [Test]
    public async Task ThrowsFileNotFoundException()
    {
        var groupUser = new GroupUser()
        {
            GroupId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };
        _groupUserRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<GroupUser?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(groupUser);
        _fileRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(File));
        
        Assert.ThrowsAsync<FileNotFoundException>(async () =>
        {
            await _commandHandler.Handle(new AddGroupShareCommand
            {
                Filename = "nonExisting.jpg",
                GroupId = groupUser.GroupId.ToString(),
                PermissionId = Permissions.readwrite,
                UserId = groupUser.UserId.ToString()
            }, CancellationToken.None);
        });
    }
}