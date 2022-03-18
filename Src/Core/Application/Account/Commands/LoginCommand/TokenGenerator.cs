using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Account.Commands.LoginCommand
{
    public class TokenGenerator
    {
        private readonly IConfigurationSection _jwt;
        private readonly string _secret;

        public TokenGenerator(IConfiguration configuration)
        {
            _jwt = configuration.GetSection("JWT");
            _secret = configuration.GetValue<string>("JWTSecret");
        }

        public async Task<JwtSecurityToken> GetToken(User user)
        {
            var secret = MD5.HashData(Encoding.UTF8.GetBytes(_secret));
            var authSigningKey = new SymmetricSecurityKey(secret);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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
