using Application.Common.Interfaces.Database;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using File = Domain.Entities.File;

namespace Infrastructure.Persistence
{
    public class JAAADbContext : DbContext, IJAAADbContext
    {
        public JAAADbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserShare> UserShares { get; set; }
        public DbSet<GroupShare> GroupShares { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);

            modelBuilder.Entity<Group>().HasKey(g => g.Id);

            modelBuilder.Entity<Permission>().HasKey(p => p.Id);

            modelBuilder.Entity<File>().HasKey(f => f.Filename);
            modelBuilder.Entity<File>().HasOne(u => u.User).WithMany(f => f.Files).HasForeignKey(uid => uid.UserId);

            modelBuilder.Entity<GroupShare>().HasKey(gs => new { gs.Filename, gs.PermissionId, gs.GroupId });
            modelBuilder.Entity<GroupShare>().HasOne(g => g.Group).WithMany(gs => gs.GroupShares).HasForeignKey(fk => fk.GroupId);
            modelBuilder.Entity<GroupShare>().HasOne(p => p.Permission).WithMany(gs => gs.GroupShares).HasForeignKey(fk => fk.PermissionId);
            modelBuilder.Entity<GroupShare>().HasOne(f => f.File).WithMany(gs => gs.GroupShares).HasForeignKey(fk => fk.Filename);

            modelBuilder.Entity<UserShare>().HasKey(us => new { us.Filename, us.PermissionId, us.UserId });
            modelBuilder.Entity<UserShare>().HasOne(u => u.User).WithMany(us => us.UserShares).HasForeignKey(fk => fk.UserId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<UserShare>().HasOne(p => p.Permission).WithMany(us => us.UserShares).HasForeignKey(fk => fk.PermissionId);
            modelBuilder.Entity<UserShare>().HasOne(f => f.File).WithMany(us => us.UserShares).HasForeignKey(fk => fk.Filename).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GroupUser>().HasKey(gu => new { gu.UserId, gu.GroupId });
            modelBuilder.Entity<GroupUser>().HasOne(u => u.User).WithMany(gu => gu.GroupUsers).HasForeignKey(fk => fk.UserId);
            modelBuilder.Entity<GroupUser>().HasOne(g => g.Group).WithMany(gu => gu.GroupUsers).HasForeignKey(fk => fk.GroupId);

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        public override DatabaseFacade Database => base.Database;

        public override EntityEntry Entry(object entity)
        {
            return base.Entry(entity);
        }
    }
}
