using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Application.Common.Interfaces.Database;
using Application.GroupShares.Commands.DeleteGroupShare;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Virtuals;
using Application.UnitTests.Common.Mocking;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.GroupShares.Commands.DeleteGroupShare;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteGroupShareCommandTests
{
    private IUnitOfWork _unitOfWork;
    private DeleteGroupShareCommand.DeleteGroupShareCommandHandler _commandHandler;
    private Mock<Repository<GroupShare>> _groupShareRepositoryMock;
    private IJAAADbContext _dbContext;

    [SetUp]
    public async Task SetUp()
    {
        _dbContext = Mock.Of<IJAAADbContext>();
        _groupShareRepositoryMock = new Mock<Repository<GroupShare>>(_dbContext);

        _unitOfWork = new UnitOfWorkMock(default, default, _groupShareRepositoryMock, default,
            default, default, default).GetMockedUnitOfWork();
        _commandHandler = new DeleteGroupShareCommand.DeleteGroupShareCommandHandler(_unitOfWork);
    }

    [Test]
    public async Task HandleDoesNotThrow()
    {
        var groupShare = new GroupShare
        {
            PermissionId = 0,
            GroupId = Guid.NewGuid(),
            Filename = "file.jpg",
        };
        
        _groupShareRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<GroupShare?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(groupShare);
        _groupShareRepositoryMock.Setup(x => x.Delete(It.IsAny<GroupShare>()));
        
        Assert.DoesNotThrowAsync(async () =>
        {
            await _commandHandler.Handle(new DeleteGroupShareCommand
            {
                Filename = groupShare.Filename,
                GroupId = groupShare.GroupId.ToString()
            }, CancellationToken.None);
        });
        
        _groupShareRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<GroupShare?, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _groupShareRepositoryMock.Verify(x => x.Delete(It.IsAny<GroupShare>()), Times.Once);
    }
    
    [Test]
    public async Task HandleThrowsShareNotFound()
    {
        _groupShareRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<GroupShare?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(GroupShare));
        _groupShareRepositoryMock.Setup(x => x.Delete(It.IsAny<GroupShare>()));
        
        Assert.ThrowsAsync<ShareNotFoundException>(async () =>
        {
            await _commandHandler.Handle(new DeleteGroupShareCommand
            {
                Filename = "notExisting.jpg",
                GroupId = Guid.NewGuid().ToString()
            }, CancellationToken.None);
        });
        
        _groupShareRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<GroupShare?, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _groupShareRepositoryMock.Verify(x => x.Delete(It.IsAny<GroupShare>()), Times.Never);
    }
}