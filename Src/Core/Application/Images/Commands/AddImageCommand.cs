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
        public class AddImageCommandHandler : IRequestHandler<AddImageCommand>
        {
            private readonly IBlobManagerService _service;
            public AddImageCommandHandler(IBlobManagerService service)
            {
                _service = service;
            }
            public async Task<Unit> Handle(AddImageCommand request, CancellationToken cancellationToken)
            {
                using (var stream = request.File.OpenReadStream())
                    await _service.AddAsync(stream, request.FileName, request.ContentType, cancellationToken);
                return Unit.Value;
            }
        }
    }
}
