using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Images.Commands
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
                    { "TargetType", request.TargetType },
                    { "TargetWidth", request.Width.Value.ToString() },
                    { "TargetHeight", request.Height.Value.ToString() },
                };
                using (var stream = request.File.OpenReadStream())
                    await _service.AddAsync(stream, "original-" + request.FileName, request.ContentType, metadata, cancellationToken);
                return Unit.Value;
            }
        }
    }
}
