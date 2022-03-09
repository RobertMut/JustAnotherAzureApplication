using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Images.Commands.UpdateImage
{
    public class DeleteImageCommand : IRequest
    {
        public string Filename { get; set; }
        public bool? DeleteMiniatures { get; set; }
        public string? Size { get; set; }

        public class DeleteImageCommandHandler : IRequestHandler<DeleteImageCommand>
        {
            private readonly IBlobManagerService _service;

            public DeleteImageCommandHandler(IBlobManagerService service)
            {
                _service = service;
            }

            public async Task<Unit> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
            {
                var filename = request.Filename.Split('-');

                string prefix = request.DeleteMiniatures.HasValue && request.DeleteMiniatures.Value ? "miniature" : "";
                string size = request.Size == "any" || string.IsNullOrEmpty(request.Size) ? "" : request.Size;
                var blobItems = await _service.GetBlobsInfoByName(prefix, size, filename[filename.Length - 1], cancellationToken);
                foreach (var blob in blobItems)
                {
                    var response = await _service.DeleteBlobAsync(blob.Name, cancellationToken);
                    if (response != 202) throw new OperationFailedException(202.ToString(), response.ToString(), nameof(DeleteImageCommandHandler));
                }

                return Unit.Value;
            }
        }
    }
}
