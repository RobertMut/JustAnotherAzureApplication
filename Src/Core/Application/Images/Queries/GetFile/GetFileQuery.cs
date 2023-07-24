using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Common.Models.File;
using Domain.Common.Helper.Filename;
using MediatR;
using FileNotFoundException = Application.Common.Exceptions.FileNotFoundException;

namespace Application.Images.Queries.GetFile;

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
    public string? UserId { get; set; }
    
    /// <summary>
    /// Determines if its miniature
    /// </summary>
    public bool IsOriginal { get; set; }
    
    /// <summary>
    /// Expected extension
    /// </summary>
    public string? ExpectedExtension { get; set; }
    
    public string ExpectedMiniatureSize { get; set; }
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
        string requestedFilename = Path.GetFileNameWithoutExtension(request.Filename);
        string filenameAfterHashing = NameHelper.GenerateHashedFilename(requestedFilename);
        string extension = string.IsNullOrEmpty(request.ExpectedExtension) || request.IsOriginal
            ? Path.GetExtension(request.Filename)
            : $".{request.ExpectedExtension}";
        string filename = string.Empty;
        
        if (request.IsOriginal)
        {
            filename = NameHelper.GenerateOriginal(request.UserId, filenameAfterHashing) + extension;
        } 
        else
        {
            filename = NameHelper.GenerateMiniature(request.UserId, request.ExpectedMiniatureSize, filenameAfterHashing) + extension;
        }

        var fileFromDatabase = await _unitOfWork.FileRepository.GetObjectBy(x => x.UserId == Guid.Parse(request.UserId) && x.Filename == filename, cancellationToken: cancellationToken);
            
        if (fileFromDatabase == null)
        {
            throw new FileNotFoundException(request.Filename);
        }

        var file = await _blobManagerService.DownloadAsync(fileFromDatabase.Filename, request.Id);

        return new FileVm
        {
            File = file
        };
    }
}