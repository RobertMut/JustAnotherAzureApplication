using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Common.Interfaces.Database
{
    public interface IJAAADbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Domain.Entities.File> Files { get; set; }
        DbSet<Group> Groups { get; set; }
        DbSet<Permission> Permissions { get; set; }
        DbSet<UserShare> UserShares { get; set; }
        DbSet<GroupShare> GroupShares { get; set; }
        DbSet<GroupUser> GroupUsers { get; set; }
        DatabaseFacade Database { get; }

        ValueTask DisposeAsync();
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

    }
}