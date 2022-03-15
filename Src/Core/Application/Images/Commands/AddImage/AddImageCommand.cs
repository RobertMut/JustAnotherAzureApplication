using Application.Common.Exceptions;
using Application.Common.Interfaces.Blob;
using Domain.Common.Helper.Enum;
using Domain.Constants.Image;
using Domain.Enums.Image;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Application.Images.Commands.AddImage
{
    public class AddImageCommand : IRequest
    {
        public IFormFile File { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public Format? TargetType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public class AddImageCommandHandler : IRequestHandler<AddImageCommand>
        {
            private readonly IBlobManagerService _service;

            public AddImageCommandHandler(IBlobManagerService service)
            {
                _service = service;
            }

            public async Task<Unit> Handle(AddImageCommand request, CancellationToken cancellationToken)
            {
                var metadata = new Dictionary<string, string>
                {
                    { Metadata.OriginalFile, request.Filename },
                    { Metadata.TargetType, !request.TargetType.HasValue ? request.ContentType : EnumHelper.GetDescriptionFromEnumValue(request.TargetType.Value) },
                    { Metadata.TargetWidth, request.Width.ToString() },
                    { Metadata.TargetHeight, request.Height.ToString() },
                };

                using (var stream = request.File.OpenReadStream())
                {
                    var statusCode = await _service.AddAsync(stream, Prefixes.OriginalImage + request.Filename, request.ContentType, metadata, cancellationToken);
                    if (statusCode == HttpStatusCode.Created)
                    {
                        return Unit.Value;
                    }
                    throw new OperationFailedException(HttpStatusCode.Created, statusCode, nameof(AddImageCommandHandler));
                }
            }
        }
    }
}
