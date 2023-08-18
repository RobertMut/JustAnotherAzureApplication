using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Image;
using Common.Images;
using Domain.Common.Helper.Filename;
using Domain.Constants.Image;
using Functions.Services;
using Functions.UnitTests.Common;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Functions.UnitTests;
[ExcludeFromCodeCoverage]
public class ImageEditorUnitTests
{
    private Mock<IBlobManagerService> _blobManagerService;
    private ISupportedImageFormats _supportedImageFormats;
    private IImageEditor _editor;
    private string _originalFilename;

    public static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            yield return new TestCaseData(new Dictionary<string, string>()
            {
                { Metadata.OriginalFile, "test.bmp" },
                { Metadata.TargetType, "png" },
                { Metadata.TargetWidth, "50" },
                { Metadata.TargetHeight, "50" },
            }, "image/Png");
            yield return new TestCaseData(new Dictionary<string, string>()
            {
                { Metadata.OriginalFile, "test.exe" },
                { Metadata.TargetType, "bmp" },
                { Metadata.TargetWidth, "100" },
                { Metadata.TargetHeight, "100" },
            }, "image/Bmp");
        }
    }

    [SetUp]
    public async Task SetUp()
    {
        _blobManagerService = new Mock<IBlobManagerService>();
        _supportedImageFormats = new FunctionImageFormats();
        _originalFilename = NameHelper.GenerateOriginal(Guid.NewGuid().ToString(), "file.png");
        _blobManagerService.Setup(x => x.AddAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<CancellationToken>()));
        _editor = new ImageEditor(_supportedImageFormats, _blobManagerService.Object);
    }

    [TestCaseSource("TestCases")]
    public async Task CreateMiniatureTest(Dictionary<string, string> metadata, string target)
    {
        using (var bitmap = new Bitmap(100, 100))
        using (var graphics = Graphics.FromImage(bitmap))
        using (var memoryStream = new MemoryStream())
        {
            graphics.Clear(Color.Green);
            bitmap.Save(memoryStream, ImageFormat.Png);
            memoryStream.Position = 0;
            var baseClient = new MockBlobBaseClient(memoryStream.Length, target, metadata);

            Assert.DoesNotThrowAsync(async () =>
            {
                string miniatureName = await _editor.Resize(baseClient, memoryStream, _originalFilename, Guid.NewGuid().ToString());
                Assert.False(string.IsNullOrEmpty(miniatureName));
            });
        }
    }
}