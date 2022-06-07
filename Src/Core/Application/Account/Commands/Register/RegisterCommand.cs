using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using Application.Common.Models.Account;
using Domain.Entities;
using MediatR;

namespace Application.Account.Commands.Register
{
    /// <summary>
    /// Class RegisterCommand
    /// </summary>
    public class RegisterCommand : IRequest
    {
        /// <summary>
        /// Username, password
        /// </summary>
        public RegisterModel RegisterModel { get; set; }

        /// <summary>
        /// Class RegisterCommandHandler
        /// </summary>
        public class RegisterCommandHandler : IRequestHandler<RegisterCommand>
        {
            private readonly IUnitOfWork _unitOfWork;

            /// <summary>
            /// Initializes new instance of <see cref="RegisterCommandHandler" /> class.
            /// </summary>
            /// <param name="unitOfWork">The unit of work</param>
            public RegisterCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            /// <summary>
            /// User register handler
            /// </summary>
            /// <param name="request">Register command with username and password</param>
            /// <param name="cancellationToken">CancellationToken</param>
            /// <returns>Unit</returns>
            /// <exception cref="DuplicatedException">When user exists</exception>
            public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
            {
                var user = await _unitOfWork.UserRepository.GetObjectBy(x => x.Username == request.RegisterModel.Username, cancellationToken: cancellationToken);
                if (user == null)
                {
                    await _unitOfWork.UserRepository.InsertAsync(new User
                    {
                        Username = request.RegisterModel.Username,
                        Password = request.RegisterModel.Password

                    }, cancellationToken);
                    await _unitOfWork.Save(cancellationToken);
                }
                else
                {
                    throw new DuplicatedException(request.RegisterModel.Username);
                }

                return Unit.Value;
            }
        }
    }
}
