using Domain.Entities;

namespace Application.Common.Interfaces.Identity
{
    public interface IUserManager
    {
        Task<string> CreateUserAsync(string username, string password, CancellationToken cancellationToken = default);

        Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<User> GetUserByNameAsync(string username, CancellationToken cancellationToken = default);
    }
}