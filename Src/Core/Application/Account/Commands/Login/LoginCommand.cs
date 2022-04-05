using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using Application.Common.Interfaces.Identity;
using Application.Common.Models.Account;
using MediatR;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Account.Commands.Login
{
    public class LoginCommand : IRequest<JwtSecurityToken>
    {
        public LoginModel LoginModel { get; set; }

        public class LoginCommandHandler : IRequestHandler<LoginCommand, JwtSecurityToken>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ITokenGenerator _tokenGenerator;

            public LoginCommandHandler(IUnitOfWork unitOfWork, ITokenGenerator tokenGenerator)
            {
                _unitOfWork = unitOfWork;
                _tokenGenerator = tokenGenerator;
            }

            public async Task<JwtSecurityToken> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                var user = await _unitOfWork.UserRepository.GetObjectBy(x => x.Username == request.LoginModel.UserName, cancellationToken);

                if (user == null) throw new UserNotFoundException(request.LoginModel.UserName);

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
