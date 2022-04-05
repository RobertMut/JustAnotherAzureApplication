using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using Application.UnitTests.Common.Fakes;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Application.GroupShares.Commands.AddGroupShare;
using Domain.Enums.Image;
using Domain.Common.Helper.Filename;

namespace Application.UnitTests.GroupShares.Commands.AddGroupShare
{
    public class AddGroupShareCommandTests
    {
        private IUnitOfWork _unitOfWork;
        private AddGroupShareCommand.AddGroupShareCommandHandler _commandHandler;

        [SetUp]
        public async Task SetUp()
        {
            _unitOfWork = new FakeUnitOfWork();
            _commandHandler = new AddGroupShareCommand.AddGroupShareCommandHandler(_unitOfWork);
        }

        [Test]
        public async Task HandleDoesNotThrow()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                var groupId = await _commandHandler.Handle(new AddGroupShareCommand
                {
                    Filename = NameHelper.GenerateMiniature(DbSets.UserId.ToString(), "300x300", NameHelper.GenerateHashedFilename("notshared.Png")),
                    GroupId = DbSets.GroupId.ToString(),
                    PermissionId = Permissions.readwrite,
                    UserId = DbSets.UserId.ToString()
                }, CancellationToken.None);

                Assert.False(string.IsNullOrEmpty(groupId));
            });
        }

        [Test]
        public async Task ThrowsFileNotFoundException()
        {
            Assert.ThrowsAsync<FileNotFoundException>(async () =>
            {
                var groupId = await _commandHandler.Handle(new AddGroupShareCommand
                {
                    Filename = DbSets.OriginalFilename,
                    GroupId = DbSets.GroupId.ToString(),
                    PermissionId = Permissions.readwrite,
                    UserId = DbSets.SecondUserId.ToString()
                }, CancellationToken.None);
            });
        }
    }
}
