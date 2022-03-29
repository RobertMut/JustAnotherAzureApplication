using Application.Common.Interfaces.Database;
using Domain.Enums.Image;
using MediatR;

namespace Application.UserShares.Commands.DeleteUserShare
{
    public class DeleteUserShareCommand : IRequest
    {
        public string UserId { get; set; }
        public string Filename { get; set; }

        public class DeleteUserShareCommandHandler : IRequestHandler<DeleteUserShareCommand>
        {
            private readonly IUnitOfWork _unitOfWork;

            public DeleteUserShareCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<Unit> Handle(DeleteUserShareCommand request, CancellationToken cancellationToken)
            {
                var userShare = await _unitOfWork.UserShareRepository.GetObjectBy(x => x.UserId == Guid.Parse(request.UserId)
                                                                                         && x.Filename == request.Filename, cancellationToken: cancellationToken);

                await _unitOfWork.UserShareRepository.Delete(userShare);
                await _unitOfWork.Save(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
