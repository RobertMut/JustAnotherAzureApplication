using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces.Database;
using Application.UserShares.Commands.DeleteUserShare;
using Application.UnitTests.Common.Fakes;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;

namespace Application.UnitTests.UserShares.Commands.DeleteUserShare
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class DeleteUserShareCommandTests
    {
        private IUnitOfWork _unitOfWork;
        private DeleteUserShareCommand.DeleteUserShareCommandHandler _commandHandler;

        [SetUp]
        public async Task SetUp()
        {
            _unitOfWork = new FakeUnitOfWork();
            _commandHandler = new DeleteUserShareCommand.DeleteUserShareCommandHandler(_unitOfWork);
        }

        [Test]
        public async Task HandleDoesNotThrow()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                var userId = await _commandHandler.Handle(new DeleteUserShareCommand
                {
                    Filename = DbSets.MiniatureFilename,
                    UserId = DbSets.SecondUserId.ToString()
                }, CancellationToken.None);
            });
        }

        [Test]
        public async Task HandleThrowsShareNotFoundException()
        {
            Assert.ThrowsAsync<ShareNotFoundException>(async () =>
            {
                var userId = await _commandHandler.Handle(new DeleteUserShareCommand
                {
                    Filename = DbSets.OriginalFilename,
                    UserId = DbSets.UserId.ToString()
                }, CancellationToken.None);
            });
        }
    }
}
