using System.Threading;
using Application.Common.Interfaces.Database;
using Application.Common.Virtuals;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.Common.Mocking;

public class UnitOfWorkMock
{
    private IUnitOfWork _unitOfWork;

    public UnitOfWorkMock(Mock<Repository<File>> fileRepository,
        Mock<Repository<Group>> groupRepository,
        Mock<Repository<GroupShare>> groupSharesRepository,
        Mock<Repository<UserShare>> userSharesRepository,
        Mock<Repository<User>> userRepository,
        Mock<Repository<GroupUser>> groupUserRepository,
        Mock<Repository<Permission>> permissionRepository)
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(x => x.FileRepository).Returns(fileRepository.Object);
        unitOfWorkMock.Setup(x => x.GroupRepository).Returns(groupRepository.Object);
        unitOfWorkMock.Setup(x => x.GroupShareRepository).Returns(groupSharesRepository.Object);
        unitOfWorkMock.Setup(x => x.UserShareRepository).Returns(userSharesRepository.Object);
        unitOfWorkMock.Setup(x => x.UserRepository).Returns(userRepository.Object);
        unitOfWorkMock.Setup(x => x.GroupUserRepository).Returns(groupUserRepository.Object);
        unitOfWorkMock.Setup(x => x.PermissionRepository).Returns(permissionRepository.Object);
        unitOfWorkMock.Setup(x => x.Save(It.IsAny<CancellationToken>()));
        _unitOfWork = new Mock<IUnitOfWork>().Object;
    }

    public IUnitOfWork GetMockedUnitOfWork() => _unitOfWork;
}