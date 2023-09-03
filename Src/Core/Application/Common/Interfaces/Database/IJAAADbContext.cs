using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using File = Domain.Entities.File;

namespace Application.Common.Interfaces.Database;

/// <summary>
/// Determines IJAAADbContext interface with its DbSets and overrides
/// </summary>
public interface IJAAADbContext
{
    /// <summary>
    /// Users dbset
    /// </summary>
    DbSet<User> Users { get; set; }
    /// <summary>
    /// Files dbset
    /// </summary>
    DbSet<File> Files { get; set; }
    /// <summary>
    /// Groups dbset
    /// </summary>
    DbSet<Group> Groups { get; set; }
    /// <summary>
    /// Permissions dbset
    /// </summary>
    DbSet<Permission> Permissions { get; set; }
    /// <summary>
    /// User Shares dbset
    /// </summary>
    DbSet<UserShare> UserShares { get; set; }
    /// <summary>
    /// Group shares dbset
    /// </summary>
    DbSet<GroupShare> GroupShares { get; set; }
    /// <summary>
    /// Group users dbset
    /// </summary>
    DbSet<GroupUser> GroupUsers { get; set; }
    DatabaseFacade Database { get; }

    ValueTask DisposeAsync();
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    DbSet<TEntity> Set<TEntity>() where TEntity : class;

}