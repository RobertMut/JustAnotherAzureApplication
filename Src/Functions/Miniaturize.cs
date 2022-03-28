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

namespace Functions
{
    public class Miniaturize
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageEditor _imageEditor;

        public Miniaturize(IUnitOfWork unitOfWork, IImageEditor imageEditor)
        {
            _unitOfWork = unitOfWork;
            _imageEditor = imageEditor;
        }

        [FunctionName("Miniaturize")]
        public async Task Run([BlobTrigger(Startup.ContainerStringSetting + Prefixes.OriginalImage + "{name}", Connection = Startup.Storage)] Stream myBlob,
            [Blob(Startup.ContainerStringSetting + Prefixes.OriginalImage + "{name}", FileAccess.Read, Connection = Startup.Storage)] BlobBaseClient blob,
            string name, ILogger log)
        {
            string[] splittedFilename = name.Split(Name.Delimiter);
            string userId = splittedFilename[^2];
            string miniature = await _imageEditor.Resize(blob, myBlob, splittedFilename[^1], userId);
            string originalFileName = $"{Prefixes.OriginalImage}{name}";
            var originalFile = await _unitOfWork.FileRepository.GetObjectBy(x => x.Filename == originalFileName);
            var miniatureFile = await _unitOfWork.FileRepository.GetObjectBy(x => x.Filename == miniature);

            if (originalFile == null)
            {
                await _unitOfWork.FileRepository.InsertAsync(new File
                {
                    Filename = originalFileName,
                    UserId = Guid.Parse(userId)
                });
            }
            if (miniatureFile == null)
            {
                await _unitOfWork.FileRepository.InsertAsync(new File
                {
                    Filename = miniature,
                    UserId = Guid.Parse(userId)
                });
            }

            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes\n");
        }
    }
}
