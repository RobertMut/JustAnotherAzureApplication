using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces.Database;
using Infrastructure.Repositories;
using Moq;
using NUnit.Framework;

namespace Infrastructure.UnitTests.Repositories;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UnitOfWorkTests
{
    IUnitOfWork _unitOfWork;

    [Test]
    public async Task UnitOfWorkCreatesNewRepositories()
    {
        var context = Mock.Of<IJAAADbContext>();
        _unitOfWork = new UnitOfWork(context);

        var fileRepository = _unitOfWork.FileRepository;
        var groupRepository = _unitOfWork.GroupRepository;
        var userRepository = _unitOfWork.UserRepository;
        var userShareRepository = _unitOfWork.UserShareRepository;
        var groupShareRepository = _unitOfWork.GroupShareRepository;
        var groupUserRepository = _unitOfWork.GroupUserRepository;
        var permissionRepository = _unitOfWork.PermissionRepository;
        
        Assert.NotNull(fileRepository);
        Assert.NotNull(groupRepository);
        Assert.NotNull(userRepository);
        Assert.NotNull(userShareRepository);
        Assert.NotNull(groupShareRepository);
        Assert.NotNull(groupUserRepository);
        Assert.NotNull(permissionRepository);
    }
}