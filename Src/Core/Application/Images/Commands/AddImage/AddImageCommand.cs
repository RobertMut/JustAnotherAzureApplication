using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Images.Commands.AddImage
{
    public class AddImageCommand : IRequest
    {
        public IFormFile File { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string? TargetType { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
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
                    { "OriginalFile", request.FileName },
                    { "TargetType", string.IsNullOrEmpty(request.TargetType) ? request.ContentType : request.TargetType },
                    { "TargetWidth", request.Width.HasValue ? request.Width.ToString() : default },
                    { "TargetHeight", request.Height.HasValue ? request.Height.ToString() : default },
                };
                using (var stream = request.File.OpenReadStream())
                {
                    var response = await _service.AddAsync(stream, "original-" + request.FileName, request.ContentType, metadata, cancellationToken);
                    if (response == 201)
                    {
                        return Unit.Value;
                    }
                    throw new OperationFailedException(201.ToString(), response.ToString(), nameof(AddImageCommandHandler));
                }
            }
        }
    }
}
