using Application.Account.Commands.LoginCommand;
using Application.Common.Models.Login;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var token = await _mediator.Send(new LoginCommand
            {
                LoginModel = model
            });

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiriation = token.ValidTo
            });
        }
    }
}
