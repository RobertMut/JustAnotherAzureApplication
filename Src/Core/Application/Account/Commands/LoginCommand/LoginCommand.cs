using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Account.Commands.LoginCommand
{
    public class LoginCommand : IRequest<JwtSecurityToken>
    {
        public LoginModel LoginModel { get; set; }

        public class LoginCommandHandler : IRequestHandler<LoginCommand, JwtSecurityToken>
        {
            private readonly IUserManager _userManager;
            private readonly TokenGenerator _tokenGenerator;

            public LoginCommandHandler(IUserManager userManager, IConfiguration configuration)
            {
                _userManager = userManager;
                _tokenGenerator = new TokenGenerator(configuration);
            }

            public async Task<JwtSecurityToken> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserByNameAsync(request.LoginModel.UserName);

                if (request.LoginModel.UserName == user.Username && request.LoginModel.Password == user.Password)
                {
                    var token = await _tokenGenerator.GetToken(user);

                    return token;
                }

                throw new UnauthorizedException(request.LoginModel.UserName);
            }
        }
    }
}
