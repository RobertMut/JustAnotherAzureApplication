using Application.Common.Interfaces.Identity;
using Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUserManager _userManager;

        public AccountsController(IUserManager userManager)
        {
            _userManager = userManager;
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.GetUserByNameAsync(model.UserName);

            if (model.UserName == user.Username && model.Password == user.Password)
            {
                var token = await _userManager.GetToken(user);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiriation = token.ValidTo
                });
            }

            return Unauthorized();
        }
    }
}
