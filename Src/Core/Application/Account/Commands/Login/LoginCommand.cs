using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using Application.Common.Interfaces.Identity;
using Application.Common.Models.Account;
using MediatR;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Account.Commands.Login
{
    /// <summary>
    /// Class LoginCommand
    /// </summary>
    public class LoginCommand : IRequest<JwtSecurityToken>
    {
        /// <summary>
        /// Username, password
        /// </summary>
        public LoginModel LoginModel { get; set; }

        /// <summary>
        /// Class LoginCommandHandler
        /// </summary>
        public class LoginCommandHandler : IRequestHandler<LoginCommand, JwtSecurityToken>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ITokenGenerator _tokenGenerator;

            /// <summary>
            /// Initializes new instance of <see cref="LoginCommandHandler" /> class.
            /// </summary>
            /// <param name="unitOfWork">The unit of work</param>
            /// <param name="tokenGenerator">Token generator class</param>
            public LoginCommandHandler(IUnitOfWork unitOfWork, ITokenGenerator tokenGenerator)
            {
                _unitOfWork = unitOfWork;
                _tokenGenerator = tokenGenerator;
            }

            /// <summary>
            /// Login handler
            /// </summary>
            /// <param name="request">Login command with username and password</param>
            /// <param name="cancellationToken">Cancellation token</param>
            /// <returns>JwtSecurityToken</returns>
            /// <exception cref="UserNotFoundException">When username not found</exception>
            /// <exception cref="UnauthorizedException">When password is wrong</exception>
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
