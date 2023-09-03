using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Application.Common.Interfaces.Database;
using Application.Common.Mappings;
using Application.UserShares.Queries.GetSharesByUser;
using AutoMapper;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Virtuals;
using Application.UnitTests.Common.Mocking;
using Domain.Entities;
using Domain.Enums.Image;
using Moq;
using Newtonsoft.Json;

namespace Application.UnitTests.UserShares.Queries.GetSharesByUser;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetSharesByUserQueryTests
{
    private IUnitOfWork _unitOfWork;
    private IMapper _mapper;
    private Mock<Repository<UserShare>> _userShareRepositoryMock;
    private IJAAADbContext _dbContext;

    [SetUp]
    public async Task SetUp()
    {
        _dbContext = Mock.Of<IJAAADbContext>();
        _userShareRepositoryMock = new Mock<Repository<UserShare>>(_dbContext);

        _unitOfWork = new UnitOfWorkMock(userSharesRepository: _userShareRepositoryMock)
            .GetMockedUnitOfWork();
        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        _mapper = configurationProvider.CreateMapper();
    }

    [Test]
    public async Task GetUserShares()
    {
        Guid userId = Guid.NewGuid();
        
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
                },
                new()
                {
                    PermissionId = 2,
                    UserId = userId,
                    Filename = "test2.jpg",
                }
            });

        var handler = new GetSharesByUserQueryHandler(_unitOfWork, _mapper);
        var query = new GetSharesByUserQuery()
        {
            UserId = userId
        };

        Assert.DoesNotThrowAsync(async () =>
        {
            var responseFromHandler = await handler.Handle(query, CancellationToken.None);
            var expected = new UserSharesListVm
            {
                Shares = new List<UserSharesDto>()
                {
                    new()
                    {
                        Filename = "test1.jpg",
                        Permissions = Permissions.write.ToString(),
                    },
                    new()
                    {
                        Filename = "test2.jpg",
                        Permissions = Permissions.read.ToString(),
                    }
                }
            };
            
            Assert.AreEqual(JsonConvert.SerializeObject(responseFromHandler), JsonConvert.SerializeObject(expected));
        });
    }
}