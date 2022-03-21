using Application.Common.Interfaces.Blob;
using MediatR;

namespace Application.Images.Queries.GetFile
{
    public class GetFileQuery : IRequest<FileVm>
    {
        public string Filename { get; set; }
        public int? Id { get; set; }
        public string UserId { get; set; }
    }

    public class GetFileQueryHandler : IRequestHandler<GetFileQuery, FileVm>
    {
        private readonly IBlobManagerService _blobManagerService;

        public GetFileQueryHandler(IBlobManagerService blobManagerService)
        {
            _blobManagerService = blobManagerService;
        }

        public async Task<FileVm> Handle(GetFileQuery request, CancellationToken cancellationToken)
        {
            string[] splittedFilename = request.Filename.Split("_");
            var file = await _blobManagerService.DownloadAsync($"{splittedFilename[0]}_{splittedFilename[^2]}_{request.UserId}_{splittedFilename[^1]}", request.Id);

            return new FileVm
            {
                File = file
            };
        }
    }
}
