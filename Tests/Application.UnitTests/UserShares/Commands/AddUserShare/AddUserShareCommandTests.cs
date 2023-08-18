using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq.Expressions;
using Application.Common.Interfaces.Database;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Virtuals;
using Application.UnitTests.Common.Mocking;
using Application.UserShares.Commands.AddUserShare;
using Domain.Enums.Image;
using Domain.Common.Helper.Filename;
using Domain.Entities;
using Moq;
using File = Domain.Entities.File;

namespace Application.UnitTests.UserShares.Commands.AddUserShare;

[ExcludeFromCodeCoverage]
[TestFixture]
public class AddUserShareCommandTests
{
    private IUnitOfWork _unitOfWork;
    private AddUserShareCommand.AddUserShareCommandHandler _commandHandler;
    private Mock<Repository<File>> _fileRepositoryMock;
    private Mock<Repository<UserShare>> _userShareRepositoryMock;
    private IJAAADbContext _dbContext;

    [SetUp]
    public async Task SetUp()
    {
        _dbContext = Mock.Of<IJAAADbContext>();
        _fileRepositoryMock = new Mock<Repository<File>>(_dbContext);
        _userShareRepositoryMock = new Mock<Repository<UserShare>>(_dbContext);
        
        _unitOfWork = new UnitOfWorkMock(_fileRepositoryMock, userSharesRepository: _userShareRepositoryMock)
            .GetMockedUnitOfWork();        
    }

    [Test]
    public async Task HandleDoesNotThrow()
    {
        _commandHandler = new AddUserShareCommand.AddUserShareCommandHandler(_unitOfWork);
        Guid userId = Guid.NewGuid();
        Guid secondUserId = Guid.NewGuid();
        string filename = NameHelper.GenerateMiniature(userId.ToString(), "300x300",
            NameHelper.GenerateHashedFilename("notshared.Png"));
        
        _fileRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new File
            {
                Filename = filename,
                OriginalName = "test.Jpeg",
                UserId = userId
            });
        _userShareRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<UserShare?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserShare
            {
                UserId = userId
            });
        
        Assert.DoesNotThrowAsync(async () =>
        {
            var result = await _commandHandler.Handle(new AddUserShareCommand
            {
                Filename = filename,
                UserId = userId.ToString(),
                PermissionId = Permissions.readwrite,
                OtherUserId = secondUserId.ToString()
            }, CancellationToken.None);

            Assert.AreEqual(result, userId.ToString());
        });
        
        _fileRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _userShareRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<UserShare?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ThrowsFileNotFoundException()
    {
        _commandHandler = new AddUserShareCommand.AddUserShareCommandHandler(_unitOfWork);
        Guid userId = Guid.NewGuid();
        Guid secondUserId = Guid.NewGuid();
        string filename = NameHelper.GenerateMiniature(userId.ToString(), "300x300",
            NameHelper.GenerateHashedFilename("notshared.Png"));
        
        _fileRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(File));

        Assert.ThrowsAsync<FileNotFoundException>(async () => await _commandHandler.Handle(new AddUserShareCommand
        {
            Filename = filename,
            UserId = userId.ToString(),
            PermissionId = Permissions.readwrite,
            OtherUserId = secondUserId.ToString()

        }, CancellationToken.None));
        
        _fileRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _userShareRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<UserShare?>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}