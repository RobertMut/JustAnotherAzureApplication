using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces.Database;
using Application.Common.Mappings;
using Application.UserShares.Queries.GetSharesByUser;
using Application.UnitTests.Common.Fakes;
using AutoMapper;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UnitTests.UserShares.Queries.GetSharesByUser;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetSharesByUserQueryTests
{
    private IUnitOfWork _unitOfWork;
    private IMapper _mapper;

    [SetUp]
    public async Task SetUp()
    {
        _unitOfWork = new FakeUnitOfWork();
        var configurationProvider = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configurationProvider.CreateMapper();
    }

    [Test]
    public async Task GetUserShares()
    {
        var handler = new GetSharesByUserQueryHandler(_unitOfWork, _mapper);
        var query = new GetSharesByUserQuery()
        {
            UserId = DbSets.SecondUserId
        };

        Assert.DoesNotThrowAsync(async () =>
        {
            var responseFromHandler = await handler.Handle(query, CancellationToken.None);

            Assert.IsInstanceOf<UserSharesListVm>(responseFromHandler);
            Assert.True(responseFromHandler.Shares.Count > 0);
        });
    }
}