using Application.Common.Exceptions;
using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
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
            private readonly IRepository<File> _fileRepository;

            public DeleteImageCommandHandler(IBlobManagerService blobManagerService, IRepository<File> fileRepository)
            {
                _blobManagerService = blobManagerService;
                _fileRepository = fileRepository;
            }

            public async Task<Unit> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
            {
                var filename = request.Filename.Split('_');

                string prefix = request.DeleteMiniatures.HasValue && request.DeleteMiniatures.Value ? "miniature" : "";
                string size = request.Size == "any" || string.IsNullOrEmpty(request.Size) ? "" : request.Size;
                var blobItems = await _blobManagerService.GetBlobsInfoByName(prefix, size, filename[^1], request.UserId, cancellationToken);
                foreach (var blob in blobItems)
                {
                    var statusCode = await _blobManagerService.DeleteBlobAsync(blob.Name, cancellationToken);
                    var file = await _fileRepository.GetByIdAsync(blob.Name, cancellationToken);

                    if(file != null)
                    {
                        await _fileRepository.RemoveAsync(file.Filename, cancellationToken);
                    }
                    if (statusCode != HttpStatusCode.Accepted) throw new OperationFailedException(HttpStatusCode.Accepted,
                        statusCode, nameof(DeleteImageCommandHandler));
                }

                return Unit.Value;
            }
        }
    }
}
