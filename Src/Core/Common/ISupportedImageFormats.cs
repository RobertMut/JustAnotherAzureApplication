using System.Drawing.Imaging;

namespace Common
{
    public interface ISupportedImageFormats
    {
        IDictionary<string, ImageFormat> FileFormat { get; }
    }
}
