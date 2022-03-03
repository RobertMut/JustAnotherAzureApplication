using Common;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace Functions
{
    public class FunctionImageFormats : ISupportedImageFormats
    {
        public IDictionary<string, ImageFormat> FileFormat => new Dictionary<string, ImageFormat>
        {
            {"png", ImageFormat.Png },
            {"jpeg", ImageFormat.Jpeg },
            {"jpg", ImageFormat.Jpeg },
            {"bmp", ImageFormat.Bmp },
            {"tiff", ImageFormat.Tiff }
        };
    }
}
