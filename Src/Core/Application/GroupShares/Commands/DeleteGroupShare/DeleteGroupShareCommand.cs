using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using MediatR;

namespace Application.GroupShares.Commands.DeleteGroupShare;

public class DeleteGroupShareCommand : IRequest
{
    /// <summary>
    /// GroupId associated with share
    /// </summary>
    public string GroupId { get; set; }
    /// <summary>
    /// Shared filename
    /// </summary>
    public string Filename { get; set; }
    
    public class DeleteGroupShareCommandHandler : IRequestHandler<DeleteGroupShareCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public DeleteGroupShareCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Deletes group share
        /// </summary>
        /// <param name="request">GroupId and filename</param>
        /// <param name="cancellationToken">
        /// <see cref="CancellationToken"/>
        /// </param>
        /// <returns>Unit</returns>
        public async Task<Unit> Handle(DeleteGroupShareCommand request, CancellationToken cancellationToken)
        {
            var groupShare = await _unitOfWork.GroupShareRepository.GetObjectBy(x => x.GroupId == Guid.Parse(request.GroupId)
                && x.Filename == request.Filename, cancellationToken: cancellationToken);

            if (groupShare == null)
            {
                throw new ShareNotFoundException(request.Filename, request.GroupId);
            }

            await _unitOfWork.GroupShareRepository.Delete(groupShare);
            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }
    }
}