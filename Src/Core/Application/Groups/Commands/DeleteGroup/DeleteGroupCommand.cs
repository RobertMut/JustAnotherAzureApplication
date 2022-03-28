using Application.Common.Interfaces.Database;
using Domain.Entities;
using MediatR;

namespace Application.Groups.Commands.DeleteGroup
{
    public class DeleteGroupCommand : IRequest
    {
        public string GroupId { get; set; }

        public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand>
        {
            private IUnitOfWork _unitOfWork;

            public DeleteGroupCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<Unit> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
            {
                var group = await _unitOfWork.GroupRepository.GetObjectBy(x => x.Id == Guid.Parse(request.GroupId), cancellationToken: cancellationToken);
                
                await _unitOfWork.GroupRepository.Delete(group);
                
                return Unit.Value;
            }
        }
    }
}
