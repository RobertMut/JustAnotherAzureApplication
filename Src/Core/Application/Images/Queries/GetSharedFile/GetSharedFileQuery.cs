using Application.Common.Exceptions;
using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Common.Models.File;
using Domain.Common.Helper.Filename;
using Domain.Constants.Image;
using MediatR;

namespace Application.Images.Queries.GetSharedFile
{
    /// <summary>
    /// Class GetSharedFileQuery
    /// </summary>
    public class GetSharedFileQuery : IRequest<FileVm>
    {
        /// <summary>
        /// Filename
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// Version id
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// User Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Other user id
        /// </summary>
        public string OtherUserId { get; set; }
        /// <summary>
        /// Group id
        /// </summary>
        public string GroupId { get; set; }
    }

    /// <summary>
    /// Class GetSharedFileQueryHandler
    /// </summary>
    public class GetSharedFileQueryHandler : IRequestHandler<GetSharedFileQuery, FileVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobManagerService _blobManagerService;

        /// <summary>
        /// Initializes new instance of <see cref="GetSharedFileQueryHandler" /> class.
        /// </summary>
        /// <param name="blobManagerService">The blob manager service</param>
        /// <param name="unitOfWork">The <see cref="IUnitOfWork"/></param>
        public GetSharedFileQueryHandler(IBlobManagerService blobManagerService, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _blobManagerService = blobManagerService;
        }

        /// <summary>
        /// Get shared file
        /// If GroupId is empty or null, tries to get file by user shares
        /// </summary>
        /// <param name="request">
        /// <see cref="GetSharedFileQuery"/>
        /// </param>
        /// <param name="cancellationToken">
        /// <see cref="CancellationToken"/>
        /// </param>
        /// <returns></returns>
        /// <exception cref="ShareNotFoundException">If share does not exists</exception>
        /// <exception cref="AccessDeniedException">If file is not shared or permissions are not enough</exception>
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
