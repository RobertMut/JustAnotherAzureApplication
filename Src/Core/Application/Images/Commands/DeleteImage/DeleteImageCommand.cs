using Application.Common.Helpers.Exception;
using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Domain.Constants.Image;
using MediatR;
using System.Net;

namespace Application.Images.Commands.DeleteImage;

public class DeleteImageCommand : IRequest
{
    /// <summary>
    /// Filename to be deleted
    /// </summary>
    public string Filename { get; set; }
    /// <summary>
    /// Determines if miniatures should be deleted or not
    /// </summary>
    public bool? DeleteMiniatures { get; set; }
    /// <summary>
    /// Size to be deleted
    /// </summary>
    public string? Size { get; set; }
    /// <summary>
    /// UserId
    /// </summary>
    public string UserId { get; set; }
    
    public class DeleteImageCommandHandler : IRequestHandler<DeleteImageCommand>
    {
        private readonly IBlobManagerService _blobManagerService;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteImageCommandHandler(IBlobManagerService blobManagerService, IUnitOfWork unitOfWork)
        {
            _blobManagerService = blobManagerService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Deletes image
        /// </summary>
        /// <param name="request">
        /// <see cref="DeleteImageCommand"/>
        /// </param>
        /// <param name="cancellationToken">
        /// <see cref="CancellationToken"/>
        /// </param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">If file does not exists</exception>
        public async Task<Unit> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
        {
            var file = await _unitOfWork.FileRepository.GetObjectBy(x => x.OriginalName == request.Filename && x.UserId == Guid.Parse(request.UserId));
            if (file == null)
            {
                throw new FileNotFoundException(request.Filename);
            }

            string filename = file.Filename.Split(Name.Delimiter)[^1];
            string prefix = request.DeleteMiniatures.HasValue && request.DeleteMiniatures.Value ? Prefixes.MiniatureImage : string.Empty;
            string size = request.Size == "any" || string.IsNullOrEmpty(request.Size) ? "" : request.Size;
            var blobItems = await _blobManagerService.GetBlobsInfoByName(prefix, size, filename, request.UserId, cancellationToken);

            foreach (var blob in blobItems)
            {
                var statusCode = await _blobManagerService.DeleteBlobAsync(blob.Name, cancellationToken);
                    
                StatusCode.Check(HttpStatusCode.Accepted, statusCode, this);

                var fileToDelete = await _unitOfWork.FileRepository.GetObjectBy(x => x.Filename == blob.Name, cancellationToken: cancellationToken);
                if (fileToDelete != null)
                {
                    await _unitOfWork.FileRepository.Delete(fileToDelete);
                    await _unitOfWork.Save(cancellationToken);
                }
            }

            return Unit.Value;
        }
    }
}