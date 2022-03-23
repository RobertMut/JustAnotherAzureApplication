using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Image;
using Azure.Storage.Blobs.Specialized;
using Common.Images;
using Domain.Common.Helper.Enum;
using Domain.Constants.Image;
using Domain.Enums.Image;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Functions.Services
{
    public class ImageEditor : IImageEditor
    {
        private readonly IBlobManagerService _service;
        private readonly ISupportedImageFormats _formats;

        public ImageEditor(ISupportedImageFormats formats, IBlobManagerService service)
        {
            _formats = formats;
            _service = service;
        }

        public async Task<string> Resize(BlobBaseClient blob, Stream stream, string name)
        {
            var metadata = blob.GetProperties().Value.Metadata;
            var targetType = metadata[Metadata.TargetType];
            int width = int.Parse(metadata[Metadata.TargetWidth]);
            int height = int.Parse(metadata[Metadata.TargetHeight]);
            var format = _formats.FileFormat[EnumHelper.GetEnumValueFromDescription<Format>(targetType)];

            using (var image = Image.FromStream(stream))
            {
                var resizedImage = new Bitmap(image, width, height);

                using (var memStream = new MemoryStream())
                {
                    resizedImage.Save(memStream, format);
                    memStream.Position = 0;
                    var convertedFormatToString = new ImageFormatConverter().ConvertToString(format);
                    string miniatureName = $"{Prefixes.MiniatureImage}{width}x{height}_{Path.GetFileNameWithoutExtension(name)}.{convertedFormatToString}";
                    await _service.AddAsync(memStream, miniatureName,
                        $"{Prefixes.ImageFormat}{convertedFormatToString}", null, new CancellationToken());

                    return miniatureName;
                }
            }
        }
    }
}
