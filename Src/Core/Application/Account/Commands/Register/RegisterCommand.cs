using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using Application.Common.Models.Account;
using Domain.Entities;
using MediatR;

namespace Application.Account.Commands.Register
{
    public class RegisterCommand : IRequest
    {
        public RegisterModel RegisterModel { get; set; }

        public class RegisterCommandHandler : IRequestHandler<RegisterCommand>
        {
            private readonly IUnitOfWork _unitOfWork;

            public RegisterCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

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
