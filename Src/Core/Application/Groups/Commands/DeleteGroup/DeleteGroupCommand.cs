using Application.Common.Interfaces.Database;
using Domain.Entities;
using MediatR;

namespace Application.Groups.Commands.DeleteGroup
{
    /// <summary>
    /// Class DeleteGroupCommand
    /// </summary>
    public class DeleteGroupCommand : IRequest
    {
        /// <summary>
        /// GroupId
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Class DeleteGroupCommandHandler
        /// </summary>
        public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand>
        {
            private IUnitOfWork _unitOfWork;

            /// <summary>
            /// Initializes new instance of <see cref="DeleteGroupCommandHandler" /> class.
            /// </summary>
            /// <param name="unitOfWork">The <see cref="IUnitOfWork"/></param>
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
                var group = await _unitOfWork.GroupRepository.GetObjectBy(x => x.Id == Guid.Parse(request.GroupId), cancellationToken: cancellationToken);
                
                await _unitOfWork.GroupRepository.Delete(group);
                await _unitOfWork.Save(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
