using Application.Common.Interfaces.Database;
using Application.Common.Interfaces.Identity;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services.Identity
{
    public class UserManagerService : IUserManager
    {
        private readonly IJAAADbContext _jaaaaDbContext;
        private readonly IConfigurationSection _jwt;

        public UserManagerService(IJAAADbContext jaaaaDbContext, IConfiguration configuration)
        {
            _jaaaaDbContext = jaaaaDbContext;
            _jwt = configuration.GetSection("JWT");
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

        public async Task<JwtSecurityToken> GetToken(User user)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.GetValue<string>("Secret")));
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(
                issuer: _jwt.GetValue<string>("ValidIssuer"),
                audience: _jwt.GetValue<string>("ValidAudience"),
                claims: authClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
