using Application.Common.Interfaces.Identity;
using Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.GetUserByNameAsync(model.UserName);
            if (model.UserName == user.Username && model.Password == user.Password) //move to login
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
