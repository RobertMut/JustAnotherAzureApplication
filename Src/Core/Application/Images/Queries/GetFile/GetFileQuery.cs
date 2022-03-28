using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Common.Models.File;
using Domain.Common.Helper.Filename;
using Domain.Constants.Image;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobManagerService _blobManagerService;

        public GetFileQueryHandler(IBlobManagerService blobManagerService, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _blobManagerService = blobManagerService;
        }

        public async Task<FileVm> Handle(GetFileQuery request, CancellationToken cancellationToken)
        {
            string[] splittedFilename = request.Filename.Split(Name.Delimiter);
            string filename = string.Empty;
            bool isOriginal = Prefixes.OriginalImage.TrimEnd(char.Parse(Name.Delimiter)) == splittedFilename[0];

            if (isOriginal)
            {
                filename = NameHelper.GenerateOriginal(request.UserId, splittedFilename[^1]);
            } 
            else
            {
                filename = NameHelper.GenerateMiniature(request.UserId, splittedFilename[^2], splittedFilename[^1]);
            }

            var file = await _blobManagerService.DownloadAsync(filename, request.Id);

            return new FileVm
            {
                File = file
            };
        }
    }
}
