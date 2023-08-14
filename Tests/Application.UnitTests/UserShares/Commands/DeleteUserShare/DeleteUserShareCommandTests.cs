using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Application.Common.Interfaces.Database;
using Application.UserShares.Commands.DeleteUserShare;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Virtuals;
using Application.UnitTests.Common.Mocking;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.UserShares.Commands.DeleteUserShare;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteUserShareCommandTests
{
    private IUnitOfWork _unitOfWork;
    private DeleteUserShareCommand.DeleteUserShareCommandHandler _commandHandler;
    private Mock<Repository<UserShare>> _userShareRepositoryMock;
    private IJAAADbContext _dbContext;

    [SetUp]
    public async Task SetUp()
    {
        _dbContext = Mock.Of<IJAAADbContext>();
        _userShareRepositoryMock = new Mock<Repository<UserShare>>(_dbContext);
        
        _unitOfWork = new UnitOfWorkMock(userSharesRepository: _userShareRepositoryMock)
            .GetMockedUnitOfWork();  
    }

    [Test]
    public async Task HandleDoesNotThrow()
    {
        Guid userId = Guid.NewGuid();
        Guid secondUserId = Guid.NewGuid();
        
        _commandHandler = new DeleteUserShareCommand.DeleteUserShareCommandHandler(_unitOfWork);
        _userShareRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<UserShare?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserShare
            {
                PermissionId = 3,
                UserId = userId,
                Filename = "test.Jpeg",
            });
        _userShareRepositoryMock.Setup(x => x.Delete(It.IsAny<UserShare?>()));
        
        Assert.DoesNotThrowAsync(async () =>
        {
            await _commandHandler.Handle(new DeleteUserShareCommand
            {
                Filename = "test.Jpeg",
                UserId = secondUserId.ToString()
            }, CancellationToken.None);
        });
        
        _userShareRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<UserShare?, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _userShareRepositoryMock.Verify(x => x.Delete(It.IsAny<UserShare?>()), Times.Once);
    }

    [Test]
    public async Task HandleThrowsShareNotFoundException()
    {
        Guid secondUserId = Guid.NewGuid();
        
        _commandHandler = new DeleteUserShareCommand.DeleteUserShareCommandHandler(_unitOfWork);
        _userShareRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<UserShare?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(UserShare));
        
        Assert.Throws<ShareNotFoundException>(async () => await _commandHandler.Handle(new DeleteUserShareCommand
        {
            Filename = "test.Jpeg",
            UserId = secondUserId.ToString()
        }, CancellationToken.None), $"Share test.Jpeg was not found for id {secondUserId.ToString()}");
        
        _userShareRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<UserShare?, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _userShareRepositoryMock.Verify(x => x.Delete(It.IsAny<UserShare?>()), Times.Never);
    }
}