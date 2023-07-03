using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using Domain.Entities;
using MediatR;

namespace Application.Groups.Commands.DeleteGroup;

public class DeleteGroupCommand : IRequest
{
    /// <summary>
    /// GroupId
    /// </summary>
    public Guid GroupId { get; set; }

    public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand>
    {
        private IUnitOfWork _unitOfWork;

        public DeleteGroupCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Deletes group
        /// </summary>
        /// <param name="request">GroupId</param>
        /// <param name="cancellationToken">
        /// <see cref="CancellationToken"/>
        /// </param>
        /// <returns>Unit</returns>
        public async Task<Unit> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            var group = await _unitOfWork.GroupRepository.GetObjectBy(x => x.Id == request.GroupId, cancellationToken: cancellationToken);

            if (group is null)
            {
                throw new GroupNotFoundException(request.GroupId);
            }
                
            await _unitOfWork.GroupRepository.Delete(group);
            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }
    }
}