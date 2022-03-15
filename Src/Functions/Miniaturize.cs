using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Drawing;
using Azure.Storage.Blobs.Specialized;
using Application.Common.Interfaces.Blob;
using Domain.Constants.Image;
using Domain.Common.Helper.Enum;
using Domain.Enums.Image;
using Common.Images;

namespace Functions
{
    public class Miniaturize
    {
        private readonly IBlobManagerService _service;
        private readonly ISupportedImageFormats _formats;
        private const string ConnectionStringSetting = "AzureWebJobsStorage";
        private const string ContainerStringSetting = "%ImagesContainer%/";

        public Miniaturize(IBlobManagerService service, ISupportedImageFormats formats)
        {
            _service = service;
            _formats = formats;
        }

        [FunctionName("Miniaturize")]
        public async Task Run([BlobTrigger(ContainerStringSetting + Prefixes.OriginalImage + "{name}", Connection = ConnectionStringSetting)] Stream myBlob,
            [Blob(ContainerStringSetting + Prefixes.OriginalImage + "{name}", FileAccess.Read, Connection = ConnectionStringSetting)] BlobBaseClient blob,
            string name, ILogger log)
        {
            var metadata = blob.GetProperties().Value.Metadata;
            var targetType = metadata[Metadata.TargetType];
            int width = int.Parse(metadata[Metadata.TargetWidth]);
            int height = int.Parse(metadata[Metadata.TargetHeight]);
            var format = _formats.FileFormat[EnumHelper.GetEnumValueFromDescription<Format>(targetType)];

            using (var image = Image.FromStream(myBlob))
            {
                var resizedImage = new Bitmap(image, width, height);
                using (var memStream = new MemoryStream())
                {
                    resizedImage.Save(memStream, format);
                    memStream.Position = 0;
                    var convertedFormatToString = new ImageFormatConverter().ConvertToString(format);

                    await _service.AddAsync(memStream, $"{Prefixes.MiniatureImage}{width}x{height}-{Path.GetFileNameWithoutExtension(name)}.{convertedFormatToString}",
                        $"{Prefixes.ImageFormat}{convertedFormatToString}", null, new CancellationToken());
                }
            }

            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes\n");
        }
    }
}
