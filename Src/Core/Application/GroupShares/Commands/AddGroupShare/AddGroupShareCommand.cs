using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using Domain.Entities;
using Domain.Enums.Image;
using MediatR;
using FileNotFoundException = Application.Common.Exceptions.FileNotFoundException;

namespace Application.GroupShares.Commands.AddGroupShare
{
    public class AddGroupShareCommand : IRequest<string>
    {
        public string GroupId { get; set; }
        public string Filename { get; set; }
        public Permissions PermissionId { get; set; }
        public string UserId { get; set; }

        public class AddGroupShareCommandHandler : IRequestHandler<AddGroupShareCommand, string>
        {
            private readonly IUnitOfWork _unitOfWork;

            public AddGroupShareCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<string> Handle(AddGroupShareCommand request, CancellationToken cancellationToken)
            {
                var userGroup = await _unitOfWork.GroupUserRepository.GetObjectBy(x => x.UserId == Guid.Parse(request.UserId) && x.GroupId == Guid.Parse(request.GroupId), cancellationToken: cancellationToken);
                var userFile = await _unitOfWork.FileRepository.GetObjectBy(x => x.UserId == Guid.Parse(request.UserId) && x.Filename == request.Filename, cancellationToken: cancellationToken);

                if (userFile == null) throw new FileNotFoundException(request.Filename);
                if (userGroup == null) throw new AccessDeniedException(request.UserId, request.GroupId);

                var groupShare = await _unitOfWork.GroupShareRepository.InsertAsync(new GroupShare
                {
                    Filename = request.Filename,
                    GroupId = Guid.Parse(request.GroupId),
                    PermissionId = (int)request.PermissionId
                }, cancellationToken);
                await _unitOfWork.Save(cancellationToken);

                return groupShare.GroupId.ToString();
            }
        }
    }
}
