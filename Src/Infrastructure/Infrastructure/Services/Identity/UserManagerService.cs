using Application.Common.Interfaces.Database;
using Application.Common.Interfaces.Identity;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Identity
{
    public class UserManagerService : IUserManager
    {
        private readonly IJAAADbContext _jaaaaDbContext;

        public UserManagerService(IJAAADbContext jaaaaDbContext)
        {
            _jaaaaDbContext = jaaaaDbContext;
        }

        public async Task<string> CreateUserAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var task = await _jaaaaDbContext.Users.AddAsync(new User
            {
                Username = username,
                Password = password,
            }, cancellationToken);

            await _jaaaaDbContext.SaveChangesAsync(cancellationToken);

            return task.Entity.Id.ToString();
        }

        public async Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _jaaaaDbContext.Users.SingleAsync(u => u.Id == Guid.Parse(userId), cancellationToken: cancellationToken);
            
            _jaaaaDbContext.Users.Remove(user);
            await _jaaaaDbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _jaaaaDbContext.Users.SingleAsync(u => u.Id == Guid.Parse(userId), cancellationToken: cancellationToken);

            return user;
        }

        public async Task<User> GetUserByNameAsync(string username, CancellationToken cancellationToken = default)
        {
            var user = await _jaaaaDbContext.Users.SingleAsync(u => u.Username == username, cancellationToken: cancellationToken);

            return user;
        }
    }
}
