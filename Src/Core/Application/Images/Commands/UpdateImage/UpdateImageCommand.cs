using Application.Common.Exceptions;
using Application.Common.Interfaces.Blob;
using Domain.Common.Helper.Enum;
using Domain.Constants.Image;
using Domain.Enums.Image;
using MediatR;
using System.Net;

namespace Application.Images.Commands.UpdateImage
{
    public class UpdateImageCommand : IRequest
    {
        public string Filename { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Format TargetType { get; set; }
        public int? Version { get; set; }
        public string UserId { get; set; }

        public class UpdateImageCommandHandler : IRequestHandler<UpdateImageCommand>
        {
            private readonly IBlobManagerService _blobManagerService;

            public UpdateImageCommandHandler(IBlobManagerService blobManagerService)
            {
                _blobManagerService = blobManagerService;
            }

            public async Task<Unit> Handle(UpdateImageCommand request, CancellationToken cancellationToken)
            {
                var metadata = new Dictionary<string, string>
                {
                    { Metadata.TargetType, EnumHelper.GetDescriptionFromEnumValue(request.TargetType) },
                    { Metadata.TargetWidth, request.Width.ToString() },
                    { Metadata.TargetHeight, request.Height.ToString() },
                };
                if (request.Version != null)
                {
                    var statusCode = await _blobManagerService.PromoteBlobVersionAsync($"{Prefixes.OriginalImage}{request.UserId}{Name.Delimiter}{request.Filename}",
                        request.Version.Value, cancellationToken);
                    if (statusCode != HttpStatusCode.Created)
                    {
                        throw new OperationFailedException(HttpStatusCode.Created, statusCode, nameof(UpdateImageCommand));
                    }
                }
                var updateStatusCode = await _blobManagerService.UpdateAsync($"{Prefixes.OriginalImage}{request.UserId}{Name.Delimiter}{request.Filename}",
                    metadata, cancellationToken);
                if (updateStatusCode == HttpStatusCode.OK)
                {
                    return Unit.Value;
                }
                throw new OperationFailedException(HttpStatusCode.OK, updateStatusCode, nameof(UpdateImageCommand));
            }
        }
    }
}
