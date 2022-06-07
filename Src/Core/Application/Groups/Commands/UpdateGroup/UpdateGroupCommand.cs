using Application.Common.Interfaces.Database;
using Domain.Entities;
using MediatR;

namespace Application.Groups.Commands.UpdateGroup
{
    /// <summary>
    /// Class UpdateGroupCommand
    /// </summary>
    public class UpdateGroupCommand : IRequest
    {
        /// <summary>
        /// Group name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Group description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Class UpdateGroupCommand
        /// </summary>
        public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand>
        {
            private IUnitOfWork _unitOfWork;

            /// <summary>
            /// Initializes new instance of <see cref="UpdateGroupCommandHandler" /> class.
            /// </summary>
            /// <param name="unitOfWork">The <see cref="IUnitOfWork"/></param>
            public UpdateGroupCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            /// <summary>
            /// Updates group
            /// </summary>
            /// <param name="request">Name and description</param>
            /// <param name="cancellationToken">
            /// <see cref="CancellationToken"/>
            /// </param>
            /// <returns>Unit</returns>
            public async Task<Unit> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
            {
                await _unitOfWork.GroupRepository.UpdateAsync(new Group
                {
                    Description = request.Description,
                    Name = request.Name
                });
                await _unitOfWork.Save(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
