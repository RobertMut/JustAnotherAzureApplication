using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Common.Models.File;
using Application.Images.Queries.GetSharedFile;
using Azure.Storage.Blobs.Models;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using API.AutomatedTests.Implementation.Common.TestingModels.Output;
using Application.Common.Virtuals;
using Application.UnitTests.Common.Mocking;
using Domain.Common.Helper.Filename;
using Domain.Entities;
using Newtonsoft.Json;
using File = Domain.Entities.File;

namespace Application.UnitTests.Images.Queries.GetSharedFile;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetSharedFileQueryTests
{
    private Mock<IBlobManagerService> _service;
    private IUnitOfWork _unitOfWork;
    private Mock<Repository<File>> _fileRepositoryMock;
    private Mock<Repository<UserShare>> _userShareRepositoryMock;
    private Mock<Repository<GroupShare>> _groupShareRepositoryMock;
    private Mock<Repository<GroupUser>> _groupUserRepositoryMock;
    private IJAAADbContext _dbContext;

    [SetUp]
    public async Task SetUp()
    {
        _dbContext = Mock.Of<IJAAADbContext>();
        _service = new Mock<IBlobManagerService>();
        _fileRepositoryMock = new Mock<Repository<File>>(_dbContext);
        _userShareRepositoryMock = new Mock<Repository<UserShare>>(_dbContext);
        _groupShareRepositoryMock = new Mock<Repository<GroupShare>>(_dbContext);
        _groupUserRepositoryMock = new Mock<Repository<GroupUser>>(_dbContext);

        _unitOfWork = new UnitOfWorkMock(_fileRepositoryMock, userSharesRepository: _userShareRepositoryMock,
                groupSharesRepository: _groupShareRepositoryMock, groupUserRepository: _groupUserRepositoryMock)
            .GetMockedUnitOfWork();
    }

    [Test]
    public async Task GetSharedFileToUser()
    {
        Guid userId = Guid.NewGuid();
        Guid userIdToShareTo = Guid.NewGuid();
        var blob = new Mock<BlobDownloadResult>();

        _service.Setup(x => x.DownloadAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(blob.Object);
        _fileRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new File
            {
                Filename = NameHelper.GenerateOriginal(userId.ToString(), NameHelper.GenerateHashedFilename("test")) +
                           ".Png",
                OriginalName = "test.Jpeg",
                UserId = userId
            });
        _userShareRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<UserShare?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserShare
            {
                PermissionId = 3,
                UserId = userId,
                Filename = "test.Jpeg",
            });

        var handler = new GetSharedFileQueryHandler(_service.Object, _unitOfWork);
        var query = new GetSharedFileQuery
        {
            Filename = "test_100x100_test_test.Jpeg",
            UserId = userId.ToString(),
            OtherUserId = userIdToShareTo.ToString(),
        };

        Assert.DoesNotThrowAsync(async () =>
        {
            var responseFromHandler = await handler.Handle(query, CancellationToken.None);
            var expected = new FileVm
            {
                File = null
            };

            Assert.AreEqual(JsonConvert.SerializeObject(responseFromHandler), JsonConvert.SerializeObject(expected));
        });
    }

    [Test]
    public async Task GetSharedFileByGroup()
    {
        Guid userId = Guid.NewGuid();
        Guid userIdToShareTo = Guid.NewGuid();
        Guid groupId = Guid.NewGuid();

        var blob = new Mock<BlobDownloadResult>();

        _service.Setup(x => x.DownloadAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(blob.Object);
        _fileRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new File
            {
                Filename = NameHelper.GenerateOriginal(userId.ToString(), NameHelper.GenerateHashedFilename("test")) +
                           ".Png",
                OriginalName = "test.Jpeg",
                UserId = userId
            });
        _groupShareRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<GroupShare?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GroupShare
            {
                PermissionId = 3,
                GroupId = groupId,
                Filename = "test.Jpeg",
            });
        _groupUserRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<GroupUser?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GroupUser
            {
                UserId = userId,
                GroupId = groupId
            });

        var handler = new GetSharedFileQueryHandler(_service.Object, _unitOfWork);
        var query = new GetSharedFileQuery
        {
            Filename = "test_100x100_test_test.Jpeg",
            UserId = userId.ToString(),
            OtherUserId = userIdToShareTo.ToString(),
            GroupId = groupId.ToString(),
        };

        Assert.DoesNotThrowAsync(async () =>
        {
            var responseFromHandler = await handler.Handle(query, CancellationToken.None);
            var expected = new FileVm
            {
                File = null
            };

            Assert.AreEqual(JsonConvert.SerializeObject(responseFromHandler), JsonConvert.SerializeObject(expected));
        });
    }

    [Test]
    public async Task GetSharedFileToUserThrowsShareNotFoundException()
    {
        Guid userId = Guid.NewGuid();
        Guid userIdToShareTo = Guid.NewGuid();

        _fileRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new File
            {
                Filename = NameHelper.GenerateOriginal(userId.ToString(), NameHelper.GenerateHashedFilename("test")) +
                           ".Png",
                OriginalName = "test.Jpeg",
                UserId = userId
            });
        _userShareRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<UserShare?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(UserShare));

        var handler = new GetSharedFileQueryHandler(_service.Object, _unitOfWork);
        var query = new GetSharedFileQuery
        {
            Filename = "test_100x100_test_test.Jpeg",
            UserId = userId.ToString(),
            OtherUserId = userIdToShareTo.ToString(),
        };

        Assert.ThrowsAsync<ShareNotFoundException>(async () => await handler.Handle(query, new CancellationToken()),
            $"Share {query.Filename} was not found for id {userIdToShareTo.ToString()}");

        _fileRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _userShareRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<UserShare?, bool>>>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task GetSharedFileByGroupThrowsAccessDenied()
    {
        Guid userId = Guid.NewGuid();
        Guid userIdToShareTo = Guid.NewGuid();
        Guid groupId = Guid.NewGuid();

        var blob = new Mock<BlobDownloadResult>();

        _service.Setup(x => x.DownloadAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(blob.Object);
        _fileRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new File
            {
                Filename = NameHelper.GenerateOriginal(userId.ToString(), NameHelper.GenerateHashedFilename("test")) +
                           ".Png",
                OriginalName = "test.Jpeg",
                UserId = userId
            });
        _groupShareRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<GroupShare?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GroupShare
            {
                PermissionId = 3,
                GroupId = groupId,
                Filename = "test.Jpeg",
            });
        _groupUserRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<GroupUser?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(GroupUser));

        var handler = new GetSharedFileQueryHandler(_service.Object, _unitOfWork);
        var query = new GetSharedFileQuery
        {
            Filename = "test_100x100_test_test.Jpeg",
            UserId = userId.ToString(),
            OtherUserId = userIdToShareTo.ToString(),
            GroupId = groupId.ToString(),
        };

        Assert.ThrowsAsync<AccessDeniedException>(async () => await handler.Handle(query, new CancellationToken()),
            $"Access denied for {query.UserId} to {query.GroupId}");

        _fileRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _userShareRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<UserShare?, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _groupShareRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<GroupShare?, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _groupUserRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<GroupUser?, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Test]
    public async Task GetSharedFileByGroupShareNotFoundException()
    {
        Guid userId = Guid.NewGuid();
        Guid userIdToShareTo = Guid.NewGuid();
        Guid groupId = Guid.NewGuid();

        var blob = new Mock<BlobDownloadResult>();

        _service.Setup(x => x.DownloadAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(blob.Object);
        _fileRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new File
            {
                Filename = NameHelper.GenerateOriginal(userId.ToString(), NameHelper.GenerateHashedFilename("test")) + ".Png",
                OriginalName = "test.Jpeg",
                UserId = userId
            });
        _groupShareRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<GroupShare?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(GroupShare));
        _groupUserRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<GroupUser?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GroupUser
            {
                UserId = userId,
                GroupId = groupId
            });

        var handler = new GetSharedFileQueryHandler(_service.Object, _unitOfWork);
        var query = new GetSharedFileQuery
        {
            Filename = "test_100x100_test_test.Jpeg",
            UserId = userId.ToString(),
            OtherUserId = userIdToShareTo.ToString(),
            GroupId = groupId.ToString(),
        };

        Assert.ThrowsAsync<ShareNotFoundException>(async () => await handler.Handle(query, new CancellationToken()));

        _fileRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<File?, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _userShareRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<UserShare?, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _groupShareRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<GroupShare?, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _groupShareRepositoryMock.Verify(x =>
            x.GetObjectBy(It.IsAny<Expression<Func<GroupShare?, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}