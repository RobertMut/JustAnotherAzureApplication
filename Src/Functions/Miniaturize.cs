using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs.Specialized;
using Domain.Constants.Image;
using Application.Common.Interfaces.Database;
using System.IO;
using File = Domain.Entities.File;
using Application.Common.Interfaces.Image;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Functions;

[ExcludeFromCodeCoverage]
public class Miniaturize
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageEditor _imageEditor;
    
    public Miniaturize(IUnitOfWork unitOfWork, IImageEditor imageEditor)
    {
        _unitOfWork = unitOfWork;
        _imageEditor = imageEditor;
    }

    /// <summary>
    /// Runs Miniaturize azure function
    /// </summary>
    /// <param name="myBlob">Blob stream</param>
    /// <param name="blob">Blob client</param>
    /// <param name="name">Blob name</param>
    /// <param name="log"><see cref="ILogger"/></param>
    [FunctionName("Miniaturize")]
    public async Task Run([BlobTrigger(Startup.ContainerStringSetting + Prefixes.OriginalImage + "{name}", Connection = Startup.Storage)] Stream myBlob,
        [Blob(Startup.ContainerStringSetting + Prefixes.OriginalImage + "{name}", FileAccess.Read, Connection = Startup.Storage)] BlobBaseClient blob,
        string name, ILogger log)
    {
        string[] splittedFilename = name.Split(Name.Delimiter);
        string userId = splittedFilename[0];
        string miniature = await _imageEditor.Resize(blob, myBlob, splittedFilename[^1], userId);
        var miniatureFile = await _unitOfWork.FileRepository.GetObjectBy(x => x.Filename == miniature);

        if (miniatureFile == null)
        {
            await _unitOfWork.FileRepository.InsertAsync(new File
            {
                Filename = miniature,
                OriginalName = blob.GetProperties().Value.Metadata[Metadata.OriginalFile],
                UserId = Guid.Parse(userId)
            });
            await _unitOfWork.Save();
        }

        log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes\n");
    }
}