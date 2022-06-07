using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Common.Models.File;
using Domain.Common.Helper.Filename;
using Domain.Constants.Image;
using MediatR;
using FileNotFoundException = Application.Common.Exceptions.FileNotFoundException;

namespace Application.Images.Queries.GetFile
{
    /// <summary>
    /// Class GetFileQuery
    /// </summary>
    public class GetFileQuery : IRequest<FileVm>
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
        /// UserId
        /// </summary>
        public string UserId { get; set; }
    }

    /// <summary>
    /// Class GetFileQueryHandler
    /// </summary>
    public class GetFileQueryHandler : IRequestHandler<GetFileQuery, FileVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobManagerService _blobManagerService;

        /// <summary>
        /// Initializes new instance of <see cref="GetFileQueryHandler" /> class.
        /// </summary>
        /// <param name="blobManagerService">The blob manager service</param>
        /// <param name="unitOfWork">The <see cref="IUnitOfWork"/></param>
        public GetFileQueryHandler(IBlobManagerService blobManagerService, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _blobManagerService = blobManagerService;
        }

        /// <summary>
        /// Gets file
        /// </summary>
        /// <param name="request">Filename, version id, userid</param>
        /// <param name="cancellationToken">
        /// <see cref="CancellationToken"/>
        /// </param>
        /// <returns>FileVm</returns>
        /// <exception cref="FileNotFoundException">If file does not exists</exception>
        public async Task<FileVm> Handle(GetFileQuery request, CancellationToken cancellationToken)
        {
            string[] splittedFilename = request.Filename.Split(Name.Delimiter);
            string filenameAfterHashing = NameHelper.GenerateHashedFilename(splittedFilename[^1]);
            string filename = string.Empty;
            bool isOriginal = Prefixes.OriginalImage.TrimEnd(char.Parse(Name.Delimiter)) == splittedFilename[0];

            

            if (isOriginal)
            {
                filename = NameHelper.GenerateOriginal(request.UserId, filenameAfterHashing);
            } 
            else
            {
                filename = NameHelper.GenerateMiniature(request.UserId, splittedFilename[^2], filenameAfterHashing);
            }

            var fileFromDatabase = await _unitOfWork.FileRepository.GetObjectBy(x => x.UserId == Guid.Parse(request.UserId) && x.Filename == filename, cancellationToken: cancellationToken);
            
            if (fileFromDatabase == null)
            {
                throw new FileNotFoundException(request.Filename);
            }

            var file = await _blobManagerService.DownloadAsync(filename, request.Id);

            return new FileVm
            {
                File = file
            };
        }
    }
}
