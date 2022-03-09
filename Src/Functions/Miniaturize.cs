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
        private readonly IBlobCreatorService _service;
        private readonly ISupportedImageFormats _formats;

        public Miniaturize(IBlobCreatorService service, ISupportedImageFormats formats)
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
            var targetType = metadata["TargetType"];
            int width = int.Parse(metadata["TargetWidth"]);
            int height = int.Parse(metadata["TargetHeight"]);
            var format = _formats.FileFormat[targetType];
            using (var image = Image.FromStream(myBlob))
            {
                var resized = new Bitmap(image, width, height);
                using (var memStream = new MemoryStream())
                {
                    resized.Save(memStream, format);
                    memStream.Position = 0;
                    var converted = new ImageFormatConverter().ConvertToString(format);
                    await _service.AddAsync(memStream, $"miniature-{width}x{height}-{Path.GetFileNameWithoutExtension(name)}.{converted}",
                        $"image/{converted}", new CancellationToken());
                }
            }
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes\n");
        }
    }
}
