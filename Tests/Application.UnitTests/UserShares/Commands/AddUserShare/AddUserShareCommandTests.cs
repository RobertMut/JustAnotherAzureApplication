using System.Diagnostics.CodeAnalysis;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using Application.UnitTests.Common.Fakes;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Application.UserShares.Commands.AddUserShare;
using Domain.Enums.Image;
using Domain.Common.Helper.Filename;

namespace Application.UnitTests.UserShares.Commands.AddUserShare;

[ExcludeFromCodeCoverage]
[TestFixture]
public class AddUserShareCommandTests
{
    private IUnitOfWork _unitOfWork;
    private AddUserShareCommand.AddUserShareCommandHandler _commandHandler;

    [SetUp]
    public async Task SetUp()
    {
        _unitOfWork = new FakeUnitOfWork();
        _commandHandler = new AddUserShareCommand.AddUserShareCommandHandler(_unitOfWork);
    }

    [Test]
    public async Task HandleDoesNotThrow()
    {
        Assert.DoesNotThrowAsync(async () =>
        {
            var userId = await _commandHandler.Handle(new AddUserShareCommand
            {
                Filename = NameHelper.GenerateMiniature(DbSets.UserId.ToString(), "300x300", NameHelper.GenerateHashedFilename("notshared.Png")),
                UserId = DbSets.UserId.ToString(),
                PermissionId = Permissions.readwrite,
                OtherUserId = DbSets.SecondUserId.ToString()
            }, CancellationToken.None);

            Assert.False(string.IsNullOrEmpty(userId));
        });
    }

    [Test]
    public async Task ThrowsFileNotFoundException()
    {
        Assert.ThrowsAsync<FileNotFoundException>(async () =>
        {
            var userId = await _commandHandler.Handle(new AddUserShareCommand
            {
                Filename = DbSets.OriginalFilename,
                UserId = DbSets.SecondUserId.ToString(),
                PermissionId = Permissions.readwrite,
                OtherUserId = DbSets.SecondUserId.ToString()

            }, CancellationToken.None) ;
        });
    }
}