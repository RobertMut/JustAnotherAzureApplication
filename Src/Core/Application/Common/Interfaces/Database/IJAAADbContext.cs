using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using File = Domain.Entities.File;

namespace Application.Common.Interfaces.Database
{
    public interface IJAAADbContext
    {
        DbSet<User> Users { get; set; }

        DbSet<File> Files { get; set; }

        DatabaseFacade Database { get; }
        DbSet<Permission> Permissions { get; set; }
        DbSet<UserShare> UserShares { get; set; }
        DbSet<GroupShare> GroupShares { get; set; }
        DbSet<GroupUser> GroupUsers { get; set; }
        DbSet<Group> Groups { get; set; }

        EntityEntry Entry(object entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}