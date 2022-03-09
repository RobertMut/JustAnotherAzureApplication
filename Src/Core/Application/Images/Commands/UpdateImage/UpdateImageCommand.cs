using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Images.Commands.UpdateImage
{
    public class UpdateImageCommand : IRequest
    {
        public string Filename { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string TargetType { get; set; }
        public int? Version { get; set; }

        public class UpdateImageCommandHandler : IRequestHandler<UpdateImageCommand>
        {
            private readonly IBlobManagerService _service;

            public UpdateImageCommandHandler(IBlobManagerService service)
            {
                _service = service;
            }

            public async Task<Unit> Handle(UpdateImageCommand request, CancellationToken cancellationToken)
            {
                var metadata = new Dictionary<string, string>
                {
                    { "TargetType", request.TargetType },
                    { "TargetWidth", request.Width.ToString() },
                    { "TargetHeight", request.Height.ToString() },
                };
                if (request.Version != null)
                {
                    var promoteResponse = await _service.PromoteBlobVersionAsync(request.Filename, request.Version.Value, cancellationToken);
                    if (promoteResponse != 201)
                    {
                        throw new OperationFailedException(201.ToString(), promoteResponse.ToString(), nameof(UpdateImageCommand));
                    }
                }
                var response = await _service.UpdateAsync(request.Filename, metadata, cancellationToken);
                if (response == 200)
                {
                    return Unit.Value;
                }
                throw new OperationFailedException(200.ToString(), response.ToString(), nameof(UpdateImageCommand));
            }
        }
    }
}
