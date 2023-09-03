using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using Application.Common.Virtuals;
using Application.Groups.Commands.AddGroup;
using Application.UnitTests.Common.Mocking;
using Domain.Entities;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.Groups.Commands.AddGroup;

[ExcludeFromCodeCoverage]
[TestFixture]
public class AddGroupTest
{
    private IJAAADbContext _dbContext;
    private IUnitOfWork _unitOfWork;
    private Mock<Repository<Group>> _groupRepositoryMock;
    private AddGroupCommand.AddGroupCommandHandler _handler;

    [SetUp]
    public async Task SetUp()
    {
        _dbContext = Mock.Of<IJAAADbContext>();
        _groupRepositoryMock = new Mock<Repository<Group>>(_dbContext);
        _unitOfWork = new UnitOfWorkMock(groupRepository: _groupRepositoryMock).GetMockedUnitOfWork();
    }

    [Test]
    public async Task AddGroupExecutes()
    {
        Guid groupId = Guid.NewGuid();
        
        _groupRepositoryMock.Setup(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<Group?, bool>>>(), It.IsAny<CancellationToken>()));
        _groupRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<Group?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Group
            {
                Id = groupId
            });

        _handler = new AddGroupCommand.AddGroupCommandHandler(_unitOfWork);
        var query = new AddGroupCommand
        {
            Name = "Newgroup",
            Description = "desc"
        };
        
        Assert.DoesNotThrowAsync(async () =>
        {
            var responseFromHandler = await _handler.Handle(query, new CancellationToken());
            
            Assert.AreEqual(groupId.ToString(), responseFromHandler);
        });
        
        _groupRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<Group?>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Test]
    public async Task AddGroupThrowsDuplicatedException()
    {
        Guid groupId = Guid.NewGuid();
        
        _groupRepositoryMock.Setup(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<Group?, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Group
        {
            Id = groupId,
            Description = "desc",
            Name = "name"
        });
        
        _handler = new AddGroupCommand.AddGroupCommandHandler(_unitOfWork);
        var query = new AddGroupCommand
        {
            Name = "name",
            Description = "desc"
        };
        
        Assert.ThrowsAsync<DuplicatedException>(async () => await _handler.Handle(query, new CancellationToken()), $"name already exist!");
        
        _groupRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<Group?>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}