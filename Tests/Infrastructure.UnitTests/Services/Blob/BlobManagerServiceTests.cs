using System.Diagnostics.CodeAnalysis;
using System.Net;
using Infrastructure.Services.Blob;
using Infrastructure.UnitTests.TestHelpers;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Infrastructure.UnitTests.Services.Blob;

[ExcludeFromCodeCoverage]
[TestFixture]
public class BlobManagerServiceTests
{
    private BlobManagerService _blobManagerService;
    private AzuriteInitializer _azuriteInitializer;
    
    [SetUp]
    public async Task SetUp()
    {
        _azuriteInitializer = new AzuriteInitializer();
        await _azuriteInitializer.CreateUnitTestContainer();
    }

    [Test]
    public async Task AddBlobTest()
    {
        byte[] bytes = { 255, 255 };
        var metadata = new Dictionary<string, string>()
        {
            { "sample", "test" }
        };
        
        _blobManagerService = new BlobManagerService(AzuriteInitializer.DefaultConnectionString,
            AzuriteInitializer.DefaultContainer);

        HttpStatusCode code = await _blobManagerService.AddAsync(new MemoryStream(bytes), "testfile.bmp", "image/bmp", metadata, new CancellationToken());
        
        Assert.True(code == HttpStatusCode.Created);

        var blobClient = await _azuriteInitializer.GetBlobClient("testfile.bmp");
        var properties = await blobClient.GetPropertiesAsync();
        var blob = await blobClient.DownloadContentAsync();

        Assert.That(JsonConvert.SerializeObject(metadata), Is.EqualTo(JsonConvert.SerializeObject(properties.Value.Metadata)));
        Assert.That(blob.Value.Content.ToArray(), Is.EqualTo(bytes));
    }
    
    [Test]
    public async Task DeleteBlobTest()
    {
        await _azuriteInitializer.CreateSampleFile("sample.bmp");
        _blobManagerService = new BlobManagerService(AzuriteInitializer.DefaultConnectionString,
            AzuriteInitializer.DefaultContainer);

        HttpStatusCode code = await _blobManagerService.DeleteBlobAsync("sample.bmp", new CancellationToken());
        
        Assert.True(code == HttpStatusCode.Accepted);
    }
    
    [Test]
    public async Task DownloadTest()
    {
        byte[] expectedBytes = { 255, 255, 255 };
        await _azuriteInitializer.CreateSampleFile("sample.bmp");
        _blobManagerService = new BlobManagerService(AzuriteInitializer.DefaultConnectionString,
            AzuriteInitializer.DefaultContainer);

        var content = await _blobManagerService.DownloadAsync("sample.bmp", null);
        
        Assert.That(content.Content.ToArray(), Is.EqualTo(expectedBytes));
    }
    
    [Test]
    public async Task DownloadTestWithVersion()
    {
        byte[] expected = { 255, 0, 255 };

        await _azuriteInitializer.CreateSampleFile("sample.bmp");
        _blobManagerService = new BlobManagerService(AzuriteInitializer.DefaultConnectionString,
            AzuriteInitializer.DefaultContainer);
        
        HttpStatusCode code = await _blobManagerService.AddAsync(new MemoryStream(expected), "sample.bmp", "image/bmp", null , new CancellationToken());
        Assert.True(code == HttpStatusCode.Created);
        
        var firstVersion = await _blobManagerService.DownloadAsync("sample.bmp", 0);
        Assert.That(firstVersion.Content.ToArray(), Is.EqualTo(expected));
    }
    
    [Test]
    public async Task SetMetadata()
    {
        byte[] expected = { 255, 0, 255 };
        var metadata = new Dictionary<string, string>()
        {
            { "sample", "test" }
        };
        
        _blobManagerService = new BlobManagerService(AzuriteInitializer.DefaultConnectionString,
            AzuriteInitializer.DefaultContainer);
        
        HttpStatusCode code = await _blobManagerService.AddAsync(new MemoryStream(expected), "sample.bmp", "image/bmp", null , new CancellationToken());
        Assert.True(code == HttpStatusCode.Created);
        
        var wwithoutMetadata = await _blobManagerService.DownloadAsync("sample.bmp", 0);
        Assert.That(wwithoutMetadata.Content.ToArray(), Is.EqualTo(expected));
        Assert.True(wwithoutMetadata.Details.Metadata.Count == 0);

        HttpStatusCode updateCode = await _blobManagerService.UpdateAsync("sample.bmp", metadata);
        Assert.True(updateCode == HttpStatusCode.OK);
        
        var withMetadata = await _blobManagerService.DownloadAsync("sample.bmp", 0);
        Assert.That(JsonConvert.SerializeObject(metadata), Is.EqualTo(JsonConvert.SerializeObject(withMetadata.Details.Metadata)));
    }
    
    [Test]
    public async Task GetsBlobInfoByName()
    {
        Guid userId = Guid.NewGuid();
        byte[] expected = { 255, 0, 255 };

        await _azuriteInitializer.CreateSampleFile($"other_50x50_{userId.ToString()}_sample.bmp");
        _blobManagerService = new BlobManagerService(AzuriteInitializer.DefaultConnectionString,
            AzuriteInitializer.DefaultContainer);
        
        HttpStatusCode firstUploadCode = await _blobManagerService.AddAsync(new MemoryStream(expected), $"miniature_100x100_{userId.ToString()}_sample.bmp", "image/bmp", null , new CancellationToken());
        Assert.True(firstUploadCode == HttpStatusCode.Created);
        HttpStatusCode secondUploadCode = await _blobManagerService.AddAsync(new MemoryStream(expected), $"miniature_110x110_{userId.ToString()}_sample.bmp", "image/bmp", null , new CancellationToken());
        Assert.True(secondUploadCode == HttpStatusCode.Created);

        var blobs = (await _blobManagerService.GetBlobsInfoByName("miniature", "100x100", "sample", userId.ToString(),
            new CancellationToken())).ToArray();
        Assert.True(blobs[0].Name == $"miniature_100x100_{userId.ToString()}_sample.bmp");
    }
    
    [TearDown]
    public async Task TearDown()
    {
        await _azuriteInitializer.Kill();
    }
}