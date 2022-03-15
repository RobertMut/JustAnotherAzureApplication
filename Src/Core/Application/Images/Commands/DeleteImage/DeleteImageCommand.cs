using Application.Common.Exceptions;
using Application.Common.Interfaces.Blob;
using MediatR;
using System.Net;

namespace Application.Images.Commands.DeleteImage
{
    public class DeleteImageCommand : IRequest
    {
        public string Filename { get; set; }
        public bool? DeleteMiniatures { get; set; }
        public string? Size { get; set; }

        public class DeleteImageCommandHandler : IRequestHandler<DeleteImageCommand>
        {
            private readonly IBlobManagerService _blobManagerService;

            public DeleteImageCommandHandler(IBlobManagerService blobManagerService)
            {
                _blobManagerService = blobManagerService;
            }

            public async Task<Unit> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
            {
                var filename = request.Filename.Split('-');

                string prefix = request.DeleteMiniatures.HasValue && request.DeleteMiniatures.Value ? "miniature" : "";
                string size = request.Size == "any" || string.IsNullOrEmpty(request.Size) ? "" : request.Size;
                var blobItems = await _blobManagerService.GetBlobsInfoByName(prefix, size, filename[filename.Length - 1], cancellationToken);
                foreach (var blob in blobItems)
                {
                    var statusCode = await _blobManagerService.DeleteBlobAsync(blob.Name, cancellationToken);
                    if (statusCode != HttpStatusCode.Accepted) throw new OperationFailedException(HttpStatusCode.Accepted,
                        statusCode, nameof(DeleteImageCommandHandler));
                }

                return Unit.Value;
            }
        }
    }
}
