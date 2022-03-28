using Application.Common.Interfaces.Database;
using Domain.Entities;
using MediatR;

namespace Application.Groups.Commands.UpdateGroup
{
    public class UpdateGroupCommand : IRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand>
        {
            private IUnitOfWork _unitOfWork;

            public UpdateGroupCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<Unit> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
            {
                await _unitOfWork.GroupRepository.UpdateAsync(new Group
                {
                    Description = request.Description,
                    Name = request.Name
                });

                return Unit.Value;
            }
        }
    }
}
