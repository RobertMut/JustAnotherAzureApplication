﻿using Application.Common.Interfaces.Database;
using Domain.Enums.Image;
using MediatR;

namespace Application.GroupShares.Commands.DeleteGroupShare
{
    public class DeleteGroupShareCommand : IRequest
    {
        public string GroupId { get; set; }
        public string Filename { get; set; }

        public class DeleteGroupShareCommandHandler : IRequestHandler<DeleteGroupShareCommand>
        {
            private readonly IUnitOfWork _unitOfWork;

            public DeleteGroupShareCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<Unit> Handle(DeleteGroupShareCommand request, CancellationToken cancellationToken)
            {
                var groupShare = await _unitOfWork.GroupShareRepository.GetObjectBy(x => x.GroupId == Guid.Parse(request.GroupId)
                                                                                         && x.Filename == request.Filename, cancellationToken: cancellationToken);

                await _unitOfWork.GroupShareRepository.Delete(groupShare);
                await _unitOfWork.Save(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
