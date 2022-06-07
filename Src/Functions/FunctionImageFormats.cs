using Common.Images;
using Domain.Enums.Image;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace Functions
{
    /// <summary>
    /// Class FunctionImageFormats
    /// </summary>
    public class FunctionImageFormats : ISupportedImageFormats
    {
        /// <summary>
        /// Supported Image formats
        /// </summary>
        public IDictionary<Format, ImageFormat> FileFormat => new Dictionary<Format, ImageFormat>
        {
            {Format.png, ImageFormat.Png },
            {Format.jpg, ImageFormat.Jpeg },
            {Format.jpeg, ImageFormat.Jpeg },
            {Format.bmp, ImageFormat.Bmp },
            {Format.tiff, ImageFormat.Tiff }
        };
    }
}
