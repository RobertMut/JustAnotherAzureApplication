using System;
using System.Collections.Generic;
using Application.Common.Interfaces.Database;
using Application.Images.Queries.GetUserFiles;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Virtuals;
using Application.UnitTests.Common.Mocking;
using Domain.Common.Helper.Filename;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.Images.Queries.GetUserFiles;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetUserFilesQueryTests
{
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
        _fileRepositoryMock = new Mock<Repository<File>>(_dbContext);
        _userShareRepositoryMock = new Mock<Repository<UserShare>>(_dbContext);
        _groupShareRepositoryMock = new Mock<Repository<GroupShare>>(_dbContext);
        _groupUserRepositoryMock = new Mock<Repository<GroupUser>>(_dbContext);

        _unitOfWork = new UnitOfWorkMock(_fileRepositoryMock, userSharesRepository: _userShareRepositoryMock,
                groupSharesRepository: _groupShareRepositoryMock, groupUserRepository: _groupUserRepositoryMock)
            .GetMockedUnitOfWork();
    }

    [Test]
    public async Task GetSharedFile()
    {
        Guid userId = Guid.NewGuid();
        Guid groupId = Guid.NewGuid();
        
        _userShareRepositoryMock.Setup(x =>
            x.GetAsync(It.IsAny<Expression<Func<UserShare?, bool>>>(),
                It.IsAny<Func<IQueryable<UserShare>, IOrderedEnumerable<UserShare>>>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()))            
            .ReturnsAsync(new List<UserShare>()
            {
                new()
                {
                    PermissionId = 3,
                    UserId = userId,
                    Filename = "test1.jpg",
                }
            });
        _fileRepositoryMock.Setup(x =>
                x.GetAsync(It.IsAny<Expression<Func<File?, bool>>>(),
                    It.IsAny<Func<IQueryable<File>, IOrderedEnumerable<File>>>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))            
            .ReturnsAsync(new List<File>()
            {
                new()
                {
                    Filename = NameHelper.GenerateOriginal(userId.ToString(), NameHelper.GenerateHashedFilename("test")) + ".Png",
                    OriginalName = "test2.Jpeg",
                    UserId = userId
                }
            });
        _groupShareRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<GroupShare?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GroupShare
                {
                    PermissionId = 3,
                    GroupId = groupId,
                    Filename = "test3.Jpeg",
                });
        _groupUserRepositoryMock.Setup(x =>
                x.GetAsync(It.IsAny<Expression<Func<GroupUser?, bool>>>(),
                    It.IsAny<Func<IQueryable<GroupUser>, IOrderedEnumerable<GroupUser>>>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))       
            .ReturnsAsync(new List<GroupUser>()
            {
                new()
                {
                    UserId = userId,
                    GroupId = groupId
                }  
            });
        
        var handler = new GetUserFilesQueryHandler(_unitOfWork);
        var query = new GetUserFilesQuery()
        {
            UserId = userId.ToString()
        };

        Assert.DoesNotThrowAsync(async () =>
        {
            var responseFromHandler = await handler.Handle(query, CancellationToken.None);

            var expected = new UserFilesListVm
            {
                Files = new List<FileDto>()
                {
                    new()
                    {
                        Filename = "test1.jpg",
                        IsOwned = false,
                        Permission = null,
                        OriginalName = "test1.jpg",
                    },
                    new()
                    {
                        Filename = "test2.Jpeg",
                        IsOwned = true,
                        Permission = null,
                        OriginalName = "test2.Jpeg",
                    },
                    new()
                    {
                        Filename = "test3.Jpeg",
                        IsOwned = false,
                        Permission = null,
                        OriginalName = "test3.Jpeg",
                    }
                }
            };

            Assert.Equals(responseFromHandler, expected);
        });
    }
}