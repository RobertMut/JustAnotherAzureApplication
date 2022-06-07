using Application.Account.Commands.Login;
using Application.Account.Commands.Register;
using Application.Common.Models.Account;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers
{
    /// <summary>
    /// Class AccountsController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes new instance of <see cref="AccountsController" /> class.
        /// </summary>
        /// <param name="mediator"><see cref="IMediator"/></param>
        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        /// <summary>
        /// Login class
        /// </summary>
        /// <param name="model">Username + password</param>
        /// <returns>Ok response with Token</returns>
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

        /// <summary>
        /// Register method
        /// </summary>
        /// <param name="model">Username + passsword</param>
        /// <returns>Ok</returns>
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
