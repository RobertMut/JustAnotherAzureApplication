using Application.Common.Interfaces.Database;
using Domain.Entities;
using Domain.Enums.Image;
using MediatR;


namespace Application.UserShares.Commands.AddUserShare;

public class AddUserShareCommand : IRequest<string>
{
    /// <summary>
    /// UserId
    /// </summary>
    public string UserId { get; set; }
    /// <summary>
    /// UserId who gets share
    /// </summary>
    public string OtherUserId { get; set; }
    /// <summary>
    /// File to be shared
    /// </summary>
    public string Filename { get; set; }
    /// <summary>
    /// Permissions
    /// </summary>
    public Permissions PermissionId { get; set; }

    public class AddUserShareCommandHandler : IRequestHandler<AddUserShareCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddUserShareCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Add User share
        /// </summary>
        /// <param name="request"><see cref="AddUserShareCommand"/></param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">If file does not exists</exception>
        public async Task<string> Handle(AddUserShareCommand request, CancellationToken cancellationToken)
        {
            var userFile = await _unitOfWork.FileRepository.GetObjectBy(x => x.UserId == Guid.Parse(request.UserId) && x.Filename == request.Filename, cancellationToken: cancellationToken);

            if (userFile == null) throw new FileNotFoundException(request.Filename);

            var userShare = await _unitOfWork.UserShareRepository.InsertAsync(new UserShare
            {
                Filename = request.Filename,
                UserId = Guid.Parse(request.OtherUserId),
                PermissionId = (int)request.PermissionId
            }, cancellationToken);
            await _unitOfWork.Save(cancellationToken);

            return userShare.UserId.ToString();
        }
    }
}