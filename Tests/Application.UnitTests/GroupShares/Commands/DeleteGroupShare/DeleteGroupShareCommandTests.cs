using Application.Common.Interfaces.Database;
using Application.GroupShares.Commands.DeleteGroupShare;
using Application.UnitTests.Common.Fakes;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UnitTests.GroupShares.Commands.DeleteGroupShare
{
    [TestFixture]
    public class DeleteGroupShareCommandTests
    {
        private IUnitOfWork _unitOfWork;
        private DeleteGroupShareCommand.DeleteGroupShareCommandHandler _commandHandler;

        [SetUp]
        public async Task SetUp()
        {
            _unitOfWork = new FakeUnitOfWork();
            _commandHandler = new DeleteGroupShareCommand.DeleteGroupShareCommandHandler(_unitOfWork);
        }

        [Test]
        public async Task HandleDoesNotThrow()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                var groupId = await _commandHandler.Handle(new DeleteGroupShareCommand
                {
                    Filename = DbSets.MiniatureFilename,
                    GroupId = DbSets.GroupId.ToString()
                }, CancellationToken.None);
            });
        }
    }
}
