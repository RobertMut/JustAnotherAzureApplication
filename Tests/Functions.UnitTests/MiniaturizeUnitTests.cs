using Application.Common.Interfaces;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Common;
using Functions.UnitTests.Common;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Functions.UnitTests
{
    public class MiniaturizeUnitTests
    {
        private Miniaturize _miniaturize;

        [SetUp]
        public void Setup()
        {
            var testStartup = new CustomTestStartup();
            var blobManager = testStartup.Builder.Services.GetRequiredService<IBlobManagerService>();
            var formats = testStartup.Builder.Services.GetRequiredService<ISupportedImageFormats>();
            _miniaturize = new Miniaturize(blobManager, formats);
        }

        //[Test]
        public void Miniaturize()
        {
            using (Bitmap bitmap = new Bitmap(100, 100))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Green);
                using(var memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream, ImageFormat.Bmp);
                    memoryStream.Position = 0;

                    //_miniaturize.Run(memoryStream,)
                }
                
            }


        }
    }
}