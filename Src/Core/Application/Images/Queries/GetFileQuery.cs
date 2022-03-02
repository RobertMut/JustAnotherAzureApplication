using Application.Common.Interfaces;
using MediatR;

namespace Application.Images.Queries
{
    public class GetFileQuery : IRequest<FileVm>
    {
        public string Filename { get; set; }
        public string Container { get; set; }
    }
    public class GetFileQueryHandler : IRequestHandler<GetFileQuery, FileVm>
    {
        private readonly IBlobManagerService _service;

        public GetFileQueryHandler(IBlobManagerService service)
        {
            _service = service;
        }

        public async Task<FileVm> Handle(GetFileQuery request, CancellationToken cancellationToken)
        {
            var response = await _service.DownloadAsync(request.Filename, request.Container);
            return new FileVm
            {
                File = response
            };
        }
    }
}
