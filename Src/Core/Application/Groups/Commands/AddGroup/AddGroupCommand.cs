using Application.Common.Interfaces.Database;
using Domain.Entities;
using MediatR;

namespace Application.Groups.Commands.AddGroup
{
    public class AddGroupCommand : IRequest<string>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public class AddGroupCommandHandler : IRequestHandler<AddGroupCommand, string>
        {
            private readonly IUnitOfWork _unitOfWork;

            public AddGroupCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<string> Handle(AddGroupCommand request, CancellationToken cancellationToken)
            {
                var group = await _unitOfWork.GroupRepository.InsertAsync(new Group
                {
                    Description = request.Description,
                    Name = request.Name
                }, cancellationToken);

                return group.Id.ToString();
            }
        }
    }
}
