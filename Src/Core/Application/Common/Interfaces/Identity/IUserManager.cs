using Domain.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Common.Interfaces.Identity
{
    public interface IUserManager
    {
        Task<string> CreateUserAsync(string username, string password, CancellationToken cancellationToken = default);
        Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
        Task<JwtSecurityToken> GetToken(User user);
        Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default);
        Task<User> GetUserByNameAsync(string username, CancellationToken cancellationToken = default);
    }
}