using Application.Common.Interfaces.Blob;
using Common.Images;
using Domain.Constants.Image;
using Functions.UnitTests.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Functions.UnitTests
{
    public class MiniaturizeUnitTests
    {
        private Miniaturize _miniaturize;
        private IBlobManagerService _blobManager;


        [SetUp]
        public void Setup()
        {
            var testStartup = new CustomTestStartup();
            _blobManager = testStartup.Builder.Services.GetRequiredService<IBlobManagerService>();
            var formats = testStartup.Builder.Services.GetRequiredService<ISupportedImageFormats>();
            _miniaturize = new Miniaturize(_blobManager, formats);
        }

        [Test]
        public async Task Miniaturize()
        {
            var metadata = new Dictionary<string, string>
                {
                    { Metadata.OriginalFile, "test.bmp" },
                    { Metadata.TargetType, "png" },
                    { Metadata.TargetWidth, "50" },
                    { Metadata.TargetHeight, "50" },
                };

            using (var bitmap = new Bitmap(100, 100))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Green);
                using (var memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream, ImageFormat.Bmp);
                    memoryStream.Position = 0;
                    await _blobManager.AddAsync(memoryStream, "test.bmp", "image/bmp", metadata, CancellationToken.None);
                    var baseClient = new MockBlobBaseClient(memoryStream.Length, "image/bmp", metadata);

                    Assert.DoesNotThrowAsync(async () =>
                    {
                        await _miniaturize.Run(memoryStream, baseClient, "test.bmp", NullLogger.Instance);
                    });

                    var processedFile = await _blobManager.DownloadAsync(Prefixes.MiniatureImage+$"{metadata[Metadata.TargetWidth]}x{metadata[Metadata.TargetHeight]}-test.Png", null);

                    Assert.NotNull(processedFile.Content);
                    Assert.True(processedFile.Details.ContentType == "image/Png");
                }
            }
        }

        [Test]
        public async Task MiniaturizeImageButCustomExtension()
        {
            var metadata = new Dictionary<string, string>
                {
                    { Metadata.OriginalFile, "test.exe" },
                    { Metadata.TargetType, "exe" },
                    { Metadata.TargetWidth, "50" },
                    { Metadata.TargetHeight, "50" },
                };

            using (var bitmap = new Bitmap(100, 100))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Green);
                using (var memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream, ImageFormat.Bmp);
                    memoryStream.Position = 0;
                    await _blobManager.AddAsync(memoryStream, "test.bmp", "image/bmp", metadata, CancellationToken.None);
                    var baseClient = new MockBlobBaseClient(memoryStream.Length, "image/bmp", metadata);

                    Assert.DoesNotThrowAsync( async () => 
                    await _miniaturize.Run(memoryStream, baseClient, "test.bmp", NullLogger.Instance));
                }
            }
        }
    }
}