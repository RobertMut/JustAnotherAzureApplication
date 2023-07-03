using Application.Common.Interfaces.Identity;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services.Identity;

public class TokenGenerator : ITokenGenerator
{
    private readonly IConfigurationSection _jwt;
    private readonly string _secret;
    
    public TokenGenerator(IConfiguration configuration)
    {
        _jwt = configuration.GetSection("JWT");
        _secret = configuration.GetValue<string>("JWTSecret");
    }

    /// <summary>
    /// Generates token for user
    /// </summary>
    /// <param name="user">User entity</param>
    /// <returns>Security token</returns>
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