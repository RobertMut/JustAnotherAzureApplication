using Application.Common.Interfaces.Database;
using Domain.Enums.Image;
using MediatR;

namespace Application.UserShares.Commands.DeleteUserShare
{
    public class DeleteUserShareCommand : IRequest
    {
        public string UserId { get; set; }
        public Permissions Permission { get; set; }
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
                string permission = ((int)request.Permission).ToString();
                var userShare = await _unitOfWork.UserShareRepository.GetObjectBy(x => x.UserId == Guid.Parse(request.UserId)
                                                                                         && x.PermissionId == (int)request.Permission
                                                                                         && x.Filename == request.Filename, cancellationToken: cancellationToken);

                await _unitOfWork.UserShareRepository.Delete(userShare);

                return Unit.Value;
            }
        }
    }
}
