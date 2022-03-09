using Application.Common.Interfaces.Blob;
using MediatR;

namespace Application.Images.Queries
{
    public class GetFileQuery : IRequest<FileVm>
    {
        public string Filename { get; set; }
        public int? Id { get; set; }
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
            var file = await _service.DownloadAsync(request.Filename, request.Id);

            return new FileVm
            {
                File = file
            };
        }
    }
}
