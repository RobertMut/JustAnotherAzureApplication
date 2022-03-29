using Application.Account.Commands.Login;
using Application.Account.Commands.Register;
using Application.Common.Models.Account;
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
            }, new CancellationToken());

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiriation = token.ValidTo
            });
        }

        [Authorize]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            await _mediator.Send(new RegisterCommand
            {
                RegisterModel = model
            }, new CancellationToken());

            return Ok();
        }
    }
}
