using Application.Common.Interfaces.Database;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly IJAAADbContext _dbContext;

        public UserRepository(IJAAADbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> AddAsync(object[] args, CancellationToken cancellationToken = default)
        {
            var task = await _dbContext.Users.AddAsync(new User
            {
                Username = (string)args[0],
                Password = (string)args[1],
            }, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return task.Entity.Id.ToString();
        }

        public async Task<User> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users.SingleAsync(u => u.Id == Guid.Parse(id), cancellationToken: cancellationToken);

            return user;
        }

        public async Task<User> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users.SingleAsync(u => u.Username == name, cancellationToken: cancellationToken);

            return user;
        }

        public async Task RemoveAsync(string id, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users.SingleAsync(u => u.Id == Guid.Parse(id), cancellationToken: cancellationToken);

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users.SingleAsync(x => x.Id == entity.Id, cancellationToken: cancellationToken);

            user = entity;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
