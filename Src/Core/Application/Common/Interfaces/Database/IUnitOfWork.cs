using Application.Common.Virtuals;
using Domain.Entities;
using File = Domain.Entities.File;

namespace Application.Common.Interfaces.Database;

/// <summary>
/// Determines IUnitOfWork interface with repositories, save and dispose.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// File repository
    /// </summary>
    Repository<File?> FileRepository { get; }
    /// <summary>
    /// Group repository
    /// </summary>
    Repository<Group?> GroupRepository { get; }
    /// <summary>
    /// Permission repository
    /// </summary>
    Repository<Permission> PermissionRepository { get; }
    /// <summary>
    /// User repository
    /// </summary>
    Repository<User> UserRepository { get; }
    /// <summary>
    /// Group Share repository
    /// </summary>
    Repository<GroupShare?> GroupShareRepository { get; }
    /// <summary>
    /// User Share repository
    /// </summary>
    Repository<UserShare?> UserShareRepository { get; }
    /// <summary>
    /// Group User repository
    /// </summary>
    Repository<GroupUser> GroupUserRepository { get; }

    void Dispose();
    Task Save(CancellationToken cancellationToken = default);
}