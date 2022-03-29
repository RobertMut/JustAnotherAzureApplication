using Application.Common.Interfaces.Database;
using Domain.Entities;
using Domain.Enums.Image;
using MediatR;

namespace Application.GroupShares.Commands.AddGroupShare
{
    public class AddGroupShareCommand : IRequest<string>
    {
        public string GroupId { get; set; }
        public string Filename { get; set; }
        public Permissions PermissionId { get; set; }

        public class AddGroupShareCommandHandler : IRequestHandler<AddGroupShareCommand, string>
        {
            private readonly IUnitOfWork _unitOfWork;

            public AddGroupShareCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<string> Handle(AddGroupShareCommand request, CancellationToken cancellationToken)
            {
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
