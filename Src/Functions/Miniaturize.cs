using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs.Specialized;
using Domain.Constants.Image;
using Application.Common.Interfaces.Database;
using System.IO;
using File = Domain.Entities.File;
using Application.Common.Interfaces.Image;

namespace Functions
{
    public class Miniaturize
    {
        private readonly IRepository<File> _fileRepository;
        private readonly IImageEditor _imageEditor;

        public Miniaturize(IRepository<File> fileRepository, IImageEditor imageEditor)
        {
            _fileRepository = fileRepository;
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
            var originalFile = await _fileRepository.GetByNameAsync($"{Prefixes.OriginalImage}{name}");
            var miniatureFile = await _fileRepository.GetByNameAsync(miniature);

            if (originalFile == null)
            {
                await _fileRepository.AddAsync(new[] { $"{Prefixes.OriginalImage}{name}", userId });
            }
            if (miniatureFile == null)
            {
                await _fileRepository.AddAsync(new[] { miniature, userId });
            }

            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes\n");
        }
    }
}
