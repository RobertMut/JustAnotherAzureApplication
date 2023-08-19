using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Application.Common.Interfaces.Database;
using Application.Common.Mappings;
using Application.GroupShares.Queries.GetSharesByGroup;
using AutoMapper;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Virtuals;
using Application.UnitTests.Common.Mocking;
using Domain.Entities;
using Moq;
using Newtonsoft.Json;

namespace Application.UnitTests.GroupShares.Queries.GetSharesByGroup;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetSharesByGroupQueryTests
{
    private IUnitOfWork _unitOfWork;
    private IMapper _mapper;
    private Mock<Repository<GroupShare>> _groupShareRepositoryMock;
    private IJAAADbContext _dbContext;

    [SetUp]
    public async Task SetUp()
    {
        _dbContext = Mock.Of<IJAAADbContext>();
        _groupShareRepositoryMock = new Mock<Repository<GroupShare>>(_dbContext);
        _unitOfWork = new UnitOfWorkMock(groupSharesRepository: _groupShareRepositoryMock).GetMockedUnitOfWork();
        var configurationProvider = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configurationProvider.CreateMapper();
    }

    [Test]
    public async Task GetGroupShares()
    {
        Guid groupId = Guid.NewGuid();
        var groupShares = new List<GroupShare>()
        {
            new()
            {
                PermissionId = 0,
                GroupId = groupId,
                Filename = "file1.jpg",
            },
            new()
            {
                PermissionId = 1,
                GroupId = groupId,
                Filename = "file2.jpg",
            }
        };
        
        _groupShareRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<GroupShare?, bool>>>(),
                It.IsAny<Func<IQueryable<GroupShare>, IOrderedEnumerable<GroupShare>>>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(groupShares);
        
        var handler = new GetSharesByGroupQueryHandler(_unitOfWork, _mapper);
        var query = new GetSharesByGroupQuery()
        {
            GroupId = groupId.ToString()
        };

        Assert.DoesNotThrowAsync(async () =>
        {
            var responseFromHandler = await handler.Handle(query, CancellationToken.None);
            
            var expected = new GroupSharesListVm
            {
                Shares = new List<GroupSharesDto>()
                {
                    new()
                    {
                        Filename = "file1.jpg",
                        Permissions = "full"
                    },
                    new()
                    {
                        Filename = "file2.jpg",
                        Permissions = "readwrite"
                    }
                }
            };

            Assert.AreEqual(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(responseFromHandler));
        });
    }
}