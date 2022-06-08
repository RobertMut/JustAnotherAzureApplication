using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces.Database;
using Application.Common.Mappings;
using Application.GroupShares.Queries.GetSharesByGroup;
using Application.UnitTests.Common.Fakes;
using AutoMapper;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UnitTests.GroupShares.Queries.GetSharesByGroup
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class GetSharesByGroupQueryTests
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
        public async Task GetGroupShares()
        {
            var handler = new GetSharesByGroupQueryHandler(_unitOfWork, _mapper);
            var query = new GetSharesByGroupQuery()
            {
                GroupId = DbSets.GroupId.ToString()
            };

            Assert.DoesNotThrowAsync(async () =>
            {
                var responseFromHandler = await handler.Handle(query, CancellationToken.None);

                Assert.IsInstanceOf<GroupSharesListVm>(responseFromHandler);
                Assert.True(responseFromHandler.Shares.Count > 0);
            });
        }
    }
}
