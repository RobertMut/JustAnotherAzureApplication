using Application.Common.Virtuals.Repository;
using Domain.Entities;
using File = Domain.Entities.File;

namespace Application.Common.Interfaces.Database
{
    public interface IUnitOfWork
    {
        Repository<File> FileRepository { get; }
        Repository<Group> GroupRepository { get; }
        Repository<Permission> PermissionRepository { get; }
        Repository<User> UserRepository { get; }
        Repository<GroupShare> GroupShareRepository { get; }
        Repository<UserShare> UserShareRepository { get; }
        Repository<GroupUser> GroupUserRepository { get; }
    }
}