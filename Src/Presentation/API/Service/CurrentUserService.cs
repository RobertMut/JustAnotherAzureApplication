using Application.Common.Interfaces.Identity;
using MediatR;
using System.Security.Claims;

namespace API.Service
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            var username = httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(username)) UserId = username;

        }

        public string UserId { get; private set; }
    }
}
