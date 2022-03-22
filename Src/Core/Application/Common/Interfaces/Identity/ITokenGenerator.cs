using Domain.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Common.Interfaces.Identity
{
    public interface ITokenGenerator
    {
        Task<JwtSecurityToken> GetToken(User user);
    }
}
