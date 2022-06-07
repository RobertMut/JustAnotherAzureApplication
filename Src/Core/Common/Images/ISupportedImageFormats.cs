using Domain.Enums.Image;
using System.Drawing.Imaging;

namespace Common.Images
{
    /// <summary>
    /// Determines ISupportedImageFormat interface to get supported image formats.
    /// </summary>
    public interface ISupportedImageFormats
    {
        /// <summary>
        /// FileFormat dictionary
        /// </summary>
        IDictionary<Format, ImageFormat> FileFormat { get; }
    }
}
