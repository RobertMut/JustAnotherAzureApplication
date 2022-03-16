using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using File = Domain.Entities.File;

namespace Application.Common.Interfaces.Database
{
    public interface IJAAADbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<File> Files { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}