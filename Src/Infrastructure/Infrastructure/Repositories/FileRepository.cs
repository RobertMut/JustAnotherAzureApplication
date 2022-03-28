using Application.Common.Interfaces.Database;
using Microsoft.EntityFrameworkCore;
using File = Domain.Entities.File;

namespace Infrastructure.Repositories
{
    public class FileRepository : IRepository<File>
    {
        private readonly IJAAADbContext _dbContext;

        public FileRepository(IJAAADbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> AddAsync(object[] args, CancellationToken cancellationToken = default)
        {
            var task = await _dbContext.Files.AddAsync(new File
            {
                Filename = (string)args[0],
                UserId = Guid.Parse((string)args[1])
            });

            await _dbContext.SaveChangesAsync(cancellationToken);

            return task.Entity.Filename.ToString();
        }

        public async Task<File> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<File> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var file = await _dbContext.Files.SingleAsync(u => u.Filename == name, cancellationToken: cancellationToken);

            return file;
        }

        public async Task RemoveAsync(string id, CancellationToken cancellationToken = default)
        {
            var file = await _dbContext.Files.SingleAsync(u => u.Filename == id, cancellationToken: cancellationToken);
            
            _dbContext.Files.Remove(file);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(File entity, CancellationToken cancellationToken = default)
        {
            var file = await _dbContext.Files.SingleAsync(u => u.Filename == entity.Filename, cancellationToken: cancellationToken);

            file = entity;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
