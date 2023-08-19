using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces.Database;
using Application.Common.Virtuals;
using Application.Groups.Commands.UpdateGroup;
using Application.UnitTests.Common.Mocking;
using Domain.Entities;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.Groups.Commands.UpdateGroup;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UpdateGroupTest
{
    private IJAAADbContext _dbContext;
    private IUnitOfWork _unitOfWork;
    private Mock<Repository<Group>> _groupRepositoryMock;
    private UpdateGroupCommand.UpdateGroupCommandHandler _handler;

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
        _groupRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Group?>()));

        _handler = new UpdateGroupCommand.UpdateGroupCommandHandler(_unitOfWork);
        var query = new UpdateGroupCommand
        {
            Name = "Newgroup",
            Description = "desc"
        };

        Assert.DoesNotThrowAsync(async () => await _handler.Handle(query, new CancellationToken()));

        _groupRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Group?>()), Times.Once);
    }
}