using Application.Common.Interfaces.Database;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using File = Domain.Entities.File;

namespace Infrastructure.Persistence;

public class JAAADbContext : DbContext, IJAAADbContext
{
    public JAAADbContext(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    /// Users dbset
    /// </summary>
    public DbSet<User> Users { get; set; }
    /// <summary>
    /// Files dbset
    /// </summary>
    public DbSet<File> Files { get; set; }
    /// <summary>
    /// Group dbset
    /// </summary>
    public DbSet<Group> Groups { get; set; }
    /// <summary>
    /// Permissions dbset
    /// </summary>
    public DbSet<Permission> Permissions { get; set; }
    /// <summary>
    /// User shares dbset
    /// </summary>
    public DbSet<UserShare> UserShares { get; set; }
    /// <summary>
    /// Group shares dbset
    /// </summary>
    public DbSet<GroupShare> GroupShares { get; set; }
    /// <summary>
    /// Group users junction dbset
    /// </summary>
    public DbSet<GroupUser> GroupUsers { get; set; }
        
    public override DatabaseFacade Database => base.Database;
        
    public override ValueTask DisposeAsync()
    {
        return base.DisposeAsync();
    }

    public override EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
    {
        return base.Entry(entity);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    public override DbSet<TEntity> Set<TEntity>()
    {
        return base.Set<TEntity>();
    }

    /// <summary>
    /// Sets entities relations and behaviour
    /// </summary>
    /// <param name="modelBuilder"><see cref="ModelBuilder"/></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<User>().Property(u => u.Id).ValueGeneratedOnAdd();
            
        modelBuilder.Entity<Group>().HasKey(g => g.Id);
        modelBuilder.Entity<Group>().HasIndex(g => g.Id).IsUnique();
        modelBuilder.Entity<Group>().Property(g => g.Id).ValueGeneratedOnAdd();

        modelBuilder.Entity<Permission>().HasKey(p => p.Id);
        modelBuilder.Entity<Permission>().Property(p => p.Id).ValueGeneratedNever();

        modelBuilder.Entity<File>().HasKey(f => f.Filename);
        modelBuilder.Entity<File>().HasOne(u => u.User).WithMany(f => f.Files).HasForeignKey(uid => uid.UserId);

        modelBuilder.Entity<GroupShare>().HasKey(gs => new { gs.Filename, gs.PermissionId, gs.GroupId });
        modelBuilder.Entity<GroupShare>().HasOne(g => g.Group).WithMany(gs => gs.GroupShares).HasForeignKey(fk => fk.GroupId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<GroupShare>().HasOne(p => p.Permission).WithMany(gs => gs.GroupShares).HasForeignKey(fk => fk.PermissionId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<GroupShare>().HasOne(f => f.File).WithMany(gs => gs.GroupShares).HasForeignKey(fk => fk.Filename).OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<UserShare>().HasKey(us => new { us.Filename, us.PermissionId, us.UserId });
        modelBuilder.Entity<UserShare>().HasOne(u => u.User).WithMany(us => us.UserShares).HasForeignKey(fk => fk.UserId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<UserShare>().HasOne(p => p.Permission).WithMany(us => us.UserShares).HasForeignKey(fk => fk.PermissionId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<UserShare>().HasOne(f => f.File).WithMany(us => us.UserShares).HasForeignKey(fk => fk.Filename).OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<GroupUser>().HasKey(gu => new { gu.UserId, gu.GroupId });
        modelBuilder.Entity<GroupUser>().HasOne(u => u.User).WithMany(gu => gu.GroupUsers).HasForeignKey(fk => fk.UserId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<GroupUser>().HasOne(g => g.Group).WithMany(gu => gu.GroupUsers).HasForeignKey(fk => fk.GroupId).OnDelete(DeleteBehavior.NoAction);

        base.OnModelCreating(modelBuilder);
    }
}