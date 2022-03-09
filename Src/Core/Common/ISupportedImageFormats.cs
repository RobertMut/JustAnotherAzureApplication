using Domain.Enums.Image;
using System.Drawing.Imaging;

namespace Common
{
    public interface ISupportedImageFormats
    {
        IDictionary<Format, ImageFormat> FileFormat { get; }
    }
}
