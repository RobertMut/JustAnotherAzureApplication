using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using Application.Common.Interfaces.Identity;
using Application.Common.Models.Login;
using Domain.Entities;
using MediatR;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Account.Commands.LoginCommand
{
    public class LoginCommand : IRequest<JwtSecurityToken>
    {
        public LoginModel LoginModel { get; set; }

        public class LoginCommandHandler : IRequestHandler<LoginCommand, JwtSecurityToken>
        {
            private readonly IRepository<User> _userRepository;
            private readonly ITokenGenerator _tokenGenerator;

            public LoginCommandHandler(IRepository<User> userRepository, ITokenGenerator tokenGenerator)
            {
                _userRepository = userRepository;
                _tokenGenerator = tokenGenerator;
            }

            public async Task<JwtSecurityToken> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetByNameAsync(request.LoginModel.UserName);

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
