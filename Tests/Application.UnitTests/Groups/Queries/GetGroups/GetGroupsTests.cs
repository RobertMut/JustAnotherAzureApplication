using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces.Database;
using Application.Common.Mappings;
using Application.Common.Virtuals;
using Application.Groups.Queries.GetGroups;
using Application.UnitTests.Common.Mocking;
using AutoMapper;
using Domain.Entities;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Application.UnitTests.Groups.Queries.GetGroups;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetGroupsTests
{
    private IJAAADbContext _dbContext;
    private IMapper _mapper;
    private IUnitOfWork _unitOfWork;
    private Mock<Repository<Group>> _groupRepositoryMock;
    private GetGroupsQueryHandler _handler;

    [SetUp]
    public async Task SetUp()
    {
        _dbContext = Mock.Of<IJAAADbContext>();
        _groupRepositoryMock = new Mock<Repository<Group>>(_dbContext);
        _unitOfWork = new UnitOfWorkMock(groupRepository: _groupRepositoryMock).GetMockedUnitOfWork();
        var configurationProvider = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configurationProvider.CreateMapper();
    }

    [Test]
    public async Task GetGroupsTest()
    {
        var groups = new List<Group>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Group1",
                Description = "Desc",
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Group2",
                Description = "Desc2"
            }
        };
        _groupRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Group?, bool>>>(),
            It.IsAny<Func<IQueryable<Group>, IOrderedEnumerable<Group>>>(), It.IsAny<string>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(groups);
        
        _handler = new GetGroupsQueryHandler(_unitOfWork, _mapper);
        
        Assert.DoesNotThrowAsync(async () =>
        {
            var responseFromHandler = await _handler.Handle(new GetGroupsQuery(), new CancellationToken());
            var expected = new GroupListVm()
            {
                Groups = new List<GroupDto>()
                {
                    new()
                    {
                        Id = groups[0].Id.ToString(),
                        Name = groups[0].Name,
                        Description = groups[0].Description
                    },
                    new()
                    {
                        Id = groups[1].Id.ToString(),
                        Name = groups[1].Name,
                        Description = groups[1].Description
                    }
                }
            };
            
            Assert.AreEqual(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(responseFromHandler));
        });
    }
}