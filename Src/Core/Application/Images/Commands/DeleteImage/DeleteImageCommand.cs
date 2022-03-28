using Application.Common.Helpers.Exception;
using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Domain.Constants.Image;
using MediatR;
using System.Net;
using File = Domain.Entities.File;

namespace Application.Images.Commands.DeleteImage
{
    public class DeleteImageCommand : IRequest
    {
        public string Filename { get; set; }
        public bool? DeleteMiniatures { get; set; }
        public string? Size { get; set; }
        public string UserId { get; set; }

        public class DeleteImageCommandHandler : IRequestHandler<DeleteImageCommand>
        {
            private readonly IBlobManagerService _blobManagerService;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteImageCommandHandler(IBlobManagerService blobManagerService, IUnitOfWork unitOfWork)
            {
                _blobManagerService = blobManagerService;
                _unitOfWork = unitOfWork;
            }

            public async Task<Unit> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
            {
                request.Filename = request.Filename.Replace(char.Parse(Name.Delimiter), '-');
                var filename = request.Filename.Split(Name.Delimiter);

                string prefix = request.DeleteMiniatures.HasValue && request.DeleteMiniatures.Value ? Prefixes.MiniatureImage : string.Empty;
                string size = request.Size == "any" || string.IsNullOrEmpty(request.Size) ? "" : request.Size;
                var blobItems = await _blobManagerService.GetBlobsInfoByName(prefix, size, filename[^1], request.UserId, cancellationToken);
                foreach (var blob in blobItems)
                {
                    var statusCode = await _blobManagerService.DeleteBlobAsync(blob.Name, cancellationToken);
                    
                    StatusCode.Check(HttpStatusCode.Accepted, statusCode, this);

                    var file = await _unitOfWork.FileRepository.GetObjectBy(x => x.Filename == blob.Name, cancellationToken: cancellationToken);
                    if (file != null)
                    {
                        await _unitOfWork.FileRepository.Delete(file);
                    }
                }

                return Unit.Value;
            }
        }
    }
}
