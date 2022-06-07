using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using Domain.Entities;
using MediatR;

namespace Application.Groups.Commands.AddGroup
{
    /// <summary>
    /// Class AddGroupCommand
    /// </summary>
    public class AddGroupCommand : IRequest<string>
    {
        /// <summary>
        /// Group name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Group Description
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Class AddGroupCommandHandler
        /// </summary>
        public class AddGroupCommandHandler : IRequestHandler<AddGroupCommand, string>
        {
            private readonly IUnitOfWork _unitOfWork;

            /// <summary>
            /// Initializes new instance of <see cref="AddGroupCommandHandler" /> class.
            /// </summary>
            /// <param name="unitOfWork">The <see cref="IUnitOfWork"/></param>
            public AddGroupCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            /// <summary>
            /// Adds group
            /// </summary>
            /// <param name="request">Name and description</param>
            /// <param name="cancellationToken">
            /// <see cref="CancellationToken"/>
            /// </param>
            /// <returns>Group id</returns>
            /// <exception cref="DuplicatedException">If group exists</exception>
            public async Task<string> Handle(AddGroupCommand request, CancellationToken cancellationToken)
            {
                var group = await _unitOfWork.GroupRepository.GetObjectBy(x => x.Name == request.Name, cancellationToken: cancellationToken);
                
                if (group is not null) throw new DuplicatedException(group.Name);

                var newGroup = await _unitOfWork.GroupRepository.InsertAsync(new Group
                {
                    Description = request.Description,
                    Name = request.Name
                }, cancellationToken);
                await _unitOfWork.Save(cancellationToken);

                return newGroup.Id.ToString();
            }
        }
    }
}
