using Application.Common.Exceptions;
using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Common.Models.File;
using Domain.Common.Helper.Filename;
using Domain.Constants.Image;
using MediatR;

namespace Application.Images.Queries.GetSharedFile
{
    public class GetSharedFileQuery : IRequest<FileVm>
    {
        public string Filename { get; set; }
        public int? Id { get; set; }
        public string UserId { get; set; }
        public string OtherUserId { get; set; }
        public string GroupId { get; set; }
    }

    public class GetSharedFileQueryHandler : IRequestHandler<GetSharedFileQuery, FileVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobManagerService _blobManagerService;

        public GetSharedFileQueryHandler(IBlobManagerService blobManagerService, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _blobManagerService = blobManagerService;
        }

        public async Task<FileVm> Handle(GetSharedFileQuery request, CancellationToken cancellationToken)
        {
            string[] splittedFilename = request.Filename.Split(Name.Delimiter);
            string filename = NameHelper.GenerateHashedFilename(splittedFilename[^1]);
            bool isOriginal = Prefixes.OriginalImage.TrimEnd(char.Parse(Name.Delimiter)) == splittedFilename[0];

            if (isOriginal)
            {
                filename = NameHelper.GenerateOriginal(request.OtherUserId, filename);
            } 
            else
            {
                filename = NameHelper.GenerateMiniature(request.OtherUserId, splittedFilename[^2], filename);
            }

            if (string.IsNullOrEmpty(request.GroupId))
            {
                var userShare = await _unitOfWork.UserShareRepository.GetObjectBy(x => x.Filename == filename && x.UserId == Guid.Parse(request.UserId) && x.PermissionId < 3, cancellationToken: cancellationToken);
                if (userShare == null) throw new ShareNotFoundException(filename, request.OtherUserId);
            }
            else
            {
                var userGroups = await _unitOfWork.GroupUserRepository.GetObjectBy(x => x.UserId == Guid.Parse(request.UserId) && x.GroupId == Guid.Parse(request.GroupId), cancellationToken: cancellationToken);
                if (userGroups == null) throw new AccessDeniedException(request.UserId, request.GroupId);

                var groupShare = await _unitOfWork.GroupShareRepository.GetObjectBy(x => x.Filename == filename && x.PermissionId < 3, cancellationToken: cancellationToken);
                if (groupShare == null) throw new ShareNotFoundException(filename, request.GroupId);
            }

            var file = await _blobManagerService.DownloadAsync(filename, request.Id);

            return new FileVm
            {
                File = file
            };
        }
    }
}
