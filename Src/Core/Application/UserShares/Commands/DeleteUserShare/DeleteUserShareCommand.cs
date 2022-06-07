using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using Domain.Enums.Image;
using MediatR;

namespace Application.UserShares.Commands.DeleteUserShare
{
    /// <summary>
    /// Initializes new instance of <see cref="DeleteUserShareCommand" /> class.
    /// </summary>
    public class DeleteUserShareCommand : IRequest
    {
        /// <summary>
        /// UserId
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// File to be deleted
        /// </summary>
        public string Filename { get; set; }
        
        /// <summary>
        /// Class DeleteUserShareCommandHandler
        /// </summary>
        public class DeleteUserShareCommandHandler : IRequestHandler<DeleteUserShareCommand>
        {
            private readonly IUnitOfWork _unitOfWork;

            /// <summary>
            /// Initializes new instance of <see cref="DeleteUserShareCommandHandler" /> class.
            /// </summary>
            /// <param name="unitOfWork"></param>
            public DeleteUserShareCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            /// <summary>
            /// Deletes users hare
            /// </summary>
            /// <param name="request">UserId and filename</param>
            /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
            /// <returns>Unit</returns>
            /// <exception cref="ShareNotFoundException">If share does not exists</exception>
            public async Task<Unit> Handle(DeleteUserShareCommand request, CancellationToken cancellationToken)
            {
                var userShare = await _unitOfWork.UserShareRepository.GetObjectBy(x => x.UserId == Guid.Parse(request.UserId)
                                                                                         && x.Filename == request.Filename, cancellationToken: cancellationToken);

                if (userShare == null) throw new ShareNotFoundException(request.Filename, request.UserId);
                
                await _unitOfWork.UserShareRepository.Delete(userShare);
                await _unitOfWork.Save(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
