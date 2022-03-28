using Application.Common.Interfaces.Database;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<File>().HasKey(f => f.Filename);
            modelBuilder.Entity<File>().HasOne(u => u.User).WithMany(f => f.Files).HasForeignKey(uid => uid.UserId);

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        public override DatabaseFacade Database => base.Database;
    }
}
