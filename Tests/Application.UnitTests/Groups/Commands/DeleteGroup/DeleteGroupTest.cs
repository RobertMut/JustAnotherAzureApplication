using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using Application.Common.Virtuals;
using Application.Groups.Commands.DeleteGroup;
using Application.Groups.Commands.UpdateGroup;
using Application.UnitTests.Common.Mocking;
using Domain.Entities;
using FluentValidation;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.Groups.Commands.DeleteGroup;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteGroupTest
{
    private IJAAADbContext _dbContext;
    private IUnitOfWork _unitOfWork;
    private Mock<Repository<Group>> _groupRepositoryMock;
    private DeleteGroupCommand.DeleteGroupCommandHandler _handler;

    [SetUp]
    public async Task SetUp()
    {
        _dbContext = Mock.Of<IJAAADbContext>();
        _groupRepositoryMock = new Mock<Repository<Group>>(_dbContext);
        _unitOfWork = new UnitOfWorkMock(groupRepository: _groupRepositoryMock).GetMockedUnitOfWork();
    }

    [Test]
    public async Task UpdateGroupExecutes()
    {
        Guid groupId = Guid.NewGuid();

        _groupRepositoryMock.Setup(x => x.Delete(It.IsAny<Group?>()));
        _groupRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<Group?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Group
            {
                Id = groupId,
                Name = "group",
                Description = "desc"
            });

        _handler = new DeleteGroupCommand.DeleteGroupCommandHandler(_unitOfWork);
        var query = new DeleteGroupCommand
        {
            GroupId = groupId.ToString()
        };

        Assert.DoesNotThrowAsync(async () => await _handler.Handle(query, new CancellationToken()));

        _groupRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<Group?, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _groupRepositoryMock.Verify(x => x.Delete(It.IsAny<Group?>()), Times.Once);
    }

    [Test]
    public async Task UpdateGroupThrowsWrongGuidFormat()
    {
        _groupRepositoryMock.Setup(x => x.Delete(It.IsAny<Group?>()));
        _groupRepositoryMock.Setup(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<Group?, bool>>>(), It.IsAny<CancellationToken>()));

        _handler = new DeleteGroupCommand.DeleteGroupCommandHandler(_unitOfWork);
        var query = new DeleteGroupCommand
        {
            GroupId = "iam-wrong-guid-format"
        };

        Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(query, new CancellationToken()),
            "Wrong guid format");

        _groupRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<Group?, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _groupRepositoryMock.Verify(x => x.Delete(It.IsAny<Group?>()), Times.Never);
    }
    
    [Test]
    public async Task UpdateGroupThrowsGroupNotFoundException()
    {
        Guid groupId = Guid.NewGuid();

        _groupRepositoryMock.Setup(x => x.Delete(It.IsAny<Group?>()));
        _groupRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<Group?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(Group));

        _handler = new DeleteGroupCommand.DeleteGroupCommandHandler(_unitOfWork);
        var query = new DeleteGroupCommand
        {
            GroupId = groupId.ToString()
        };

        Assert.ThrowsAsync<GroupNotFoundException>(async () => await _handler.Handle(query, new CancellationToken()), $"Group with {groupId.ToString()} not found!");

        _groupRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<Group?, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _groupRepositoryMock.Verify(x => x.Delete(It.IsAny<Group?>()), Times.Never);
    }
}