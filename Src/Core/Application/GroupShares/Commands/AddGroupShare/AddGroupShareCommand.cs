using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using Domain.Entities;
using Domain.Enums.Image;
using MediatR;
using FileNotFoundException = Application.Common.Exceptions.FileNotFoundException;

namespace Application.GroupShares.Commands.AddGroupShare
{
    /// <summary>
    /// Class AddGroupShareCommand
    /// </summary>
    public class AddGroupShareCommand : IRequest<string>
    {
        /// <summary>
        /// Group id
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// Filename to be shared
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// Share permission
        /// </summary>
        public Permissions PermissionId { get; set; }
        /// <summary>
        /// Sharing user
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Class AddGroupShareCommandHandler
        /// </summary>
        public class AddGroupShareCommandHandler : IRequestHandler<AddGroupShareCommand, string>
        {
            private readonly IUnitOfWork _unitOfWork;

            /// <summary>
            /// Initializes new instance of <see cref="AddGroupShareCommandHandler" /> class.
            /// </summary>
            /// <param name="unitOfWork">The <see cref="IUnitOfWork"/></param>
            public AddGroupShareCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            
            /// <summary>
            /// Adds group share
            /// </summary>
            /// <param name="request">GroupId, Filename, PermissionId, UserId</param>
            /// <param name="cancellationToken">
            /// <see cref="CancellationToken"/>
            /// </param>
            /// <returns>GroupId</returns>
            /// <exception cref="FileNotFoundException">If file does not exists</exception>
            /// <exception cref="AccessDeniedException">If file can't be accessed</exception>
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
