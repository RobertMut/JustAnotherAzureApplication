using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Application.Common.Interfaces.Database;
using Application.Common.Virtuals;
using Domain.Entities;
using Moq;

namespace Functions.UnitTests.Common.Mocking;
[ExcludeFromCodeCoverage]
public class UnitOfWorkMock
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkMock(Mock<Repository<File>>? fileRepository = default,
        Mock<Repository<Group>>? groupRepository = default,
        Mock<Repository<GroupShare>>? groupSharesRepository = default,
        Mock<Repository<UserShare>>? userSharesRepository = default,
        Mock<Repository<User>>? userRepository = default,
        Mock<Repository<GroupUser>>? groupUserRepository = default,
        Mock<Repository<Permission>>? permissionRepository = default)
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        if (fileRepository != null) unitOfWorkMock.Setup(x => x.FileRepository).Returns(fileRepository.Object);

        if (groupRepository != null) unitOfWorkMock.Setup(x => x.GroupRepository).Returns(groupRepository.Object);

        if (groupSharesRepository != null)
            unitOfWorkMock.Setup(x => x.GroupShareRepository).Returns(groupSharesRepository.Object);

        if (userSharesRepository != null)
            unitOfWorkMock.Setup(x => x.UserShareRepository).Returns(userSharesRepository.Object);

        if (userRepository != null) unitOfWorkMock.Setup(x => x.UserRepository).Returns(userRepository.Object);

        if (groupUserRepository != null)
            unitOfWorkMock.Setup(x => x.GroupUserRepository).Returns(groupUserRepository.Object);

        if (permissionRepository != null)
            unitOfWorkMock.Setup(x => x.PermissionRepository).Returns(permissionRepository.Object);

        unitOfWorkMock.Setup(x => x.Save(It.IsAny<CancellationToken>()));
        _unitOfWork = unitOfWorkMock.Object;
    }

    public IUnitOfWork GetMockedUnitOfWork() => _unitOfWork;
}