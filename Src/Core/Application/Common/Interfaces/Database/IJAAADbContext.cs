using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using File = Domain.Entities.File;

namespace Application.Common.Interfaces.Database
{
    public interface IJAAADbContext
    {
        DbSet<User> Users { get; set; }

        DbSet<File> Files { get; set; }

        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}