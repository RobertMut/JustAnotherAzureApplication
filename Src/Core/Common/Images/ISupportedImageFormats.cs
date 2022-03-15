using Domain.Enums.Image;
using System.Drawing.Imaging;

namespace Common.Images
{
    public interface ISupportedImageFormats
    {
        IDictionary<Format, ImageFormat> FileFormat { get; }
    }
}
