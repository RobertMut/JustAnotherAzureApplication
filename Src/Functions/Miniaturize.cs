using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs.Specialized;
using Domain.Constants.Image;
using Application.Common.Interfaces.Database;
using System.Linq;
using System.IO;
using File = Domain.Entities.File;
using System;

namespace Functions
{
    public class Miniaturize
    {
        private readonly IJAAADbContext _jaaaDbContext;
        private readonly ImageEditor _imageEditor;

        public Miniaturize(IJAAADbContext jaaaDbContext, ImageEditor imageEditor)
        {
            _jaaaDbContext = jaaaDbContext;
            _imageEditor = imageEditor;
        }

        [FunctionName("Miniaturize")]
        public async Task Run([BlobTrigger(Startup.ContainerStringSetting + Prefixes.OriginalImage + "{name}", Connection = Startup.Storage)] Stream myBlob,
            [Blob(Startup.ContainerStringSetting + Prefixes.OriginalImage + "{name}", FileAccess.Read, Connection = Startup.Storage)] BlobBaseClient blob,
            string name, ILogger log)
        {
            string[] splittedFilename = name.Split("_");
            string userId = splittedFilename[^2];
            string miniature = await _imageEditor.Resize(blob, myBlob, name);
            var originalFile = _jaaaDbContext.Files.FirstOrDefault(x => x.Filename == $"{Prefixes.OriginalImage}{name}");
            var miniatureFile = _jaaaDbContext.Files.FirstOrDefault(x => x.Filename == miniature);

            if (originalFile == null)
            {
                _jaaaDbContext.Files.Add(new File
                {
                    Filename = $"{Prefixes.OriginalImage}{name}",
                    UserId = Guid.Parse(userId)
                });
            }
            if (miniatureFile == null)
            {
                _jaaaDbContext.Files.Add(new File
                {
                    Filename = miniature,
                    UserId = Guid.Parse(userId)
                });
            }

            await _jaaaDbContext.SaveChangesAsync();
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes\n");
        }
    }
}
