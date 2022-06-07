using Application.Common.Interfaces.Identity;
using System.Security.Claims;

namespace API.Service
{
    /// <summary>
    /// Class CurrentUserService
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        /// <summary>
        /// Initializes new instance of <see cref="CurrentUserService" /> class.
        /// </summary>
        /// <param name="httpContextAccessor"><see cref="IHttpContextAccessor"/></param>
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
}
