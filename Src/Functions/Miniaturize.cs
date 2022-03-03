using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Drawing;
using Azure.Storage.Blobs.Specialized;
using Common;

namespace Functions
{
    public class Miniaturize
    {
        private readonly IBlobManagerService _service;
        private readonly ISupportedImageFormats _formats;

        public Miniaturize(IBlobManagerService service, ISupportedImageFormats formats)
        {
            _service = service;
            _formats = formats;
        }
        [FunctionName("Miniaturize")]
        public async Task Run([BlobTrigger("%ImagesContainer%/original-{name}", Connection = "AzureWebJobsStorage")] Stream myBlob,
            [Blob("%ImagesContainer%/original-{name}", FileAccess.Read, Connection = "AzureWebJobsStorage")] BlobBaseClient blob,
            string name, ILogger log)
        {
            var metadata = blob.GetProperties().Value.Metadata;
            string targetType = metadata["TargetType"];
            using (var image = Image.FromStream(myBlob))
            {
                var resized = new Bitmap(image, int.Parse(metadata["TargetWidth"]), int.Parse(metadata["TargetHeight"]));
                using (var memStream = new MemoryStream())
                {
                    resized.Save(memStream, _formats.FileFormat[targetType]);
                    memStream.Position = 0;
                    await _service.AddAsync(memStream, $"miniature-{Path.GetFileNameWithoutExtension(name)}.{targetType}",
                        $"image/{targetType}", new CancellationToken());
                }
            }
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes\n");
        }
    }
}
