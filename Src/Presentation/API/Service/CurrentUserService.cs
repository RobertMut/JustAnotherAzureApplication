using Application.Common.Interfaces.Identity;
using System.Security.Claims;

namespace API.Service;


public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var username = httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(username))
        {
            UserId = username;
        }
    }

    /// <summary>
    /// User guid
    /// </summary>
    public string UserId { get; private set; }
}