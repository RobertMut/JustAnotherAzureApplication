using Application.Common.Interfaces.Database;
using Domain.Entities;
using Domain.Enums.Image;
using MediatR;

namespace Application.UserShares.Commands.AddUserShare
{
    public class AddUserShareCommand : IRequest<string>
    {
        public string UserId { get; set; }
        public string Filename { get; set; }
        public Permissions PermissionId { get; set; }

        public class AddUserShareCommandHandler : IRequestHandler<AddUserShareCommand, string>
        {
            private readonly IUnitOfWork _unitOfWork;

            public AddUserShareCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<string> Handle(AddUserShareCommand request, CancellationToken cancellationToken)
            {
                var userShare = await _unitOfWork.UserShareRepository.InsertAsync(new UserShare
                {
                    Filename = request.Filename,
                    UserId = Guid.Parse(request.UserId),
                    PermissionId = (int)request.PermissionId
                }, cancellationToken);
                await _unitOfWork.Save(cancellationToken);

                return userShare.UserId.ToString();
            }
        }
    }
}
