using Domain.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Common.Interfaces.Identity;

/// <summary>
/// Determines ITokenGenerator Interface with GetToken to generate jwt tokens.
/// </summary>
public interface ITokenGenerator
{
    /// <summary>
    /// Generates token
    /// </summary>
    /// <param name="user">Id, Username, Password</param>
    /// <returns>SecurityToken</returns>
    Task<JwtSecurityToken> GetToken(User user);
}