using System.Diagnostics.CodeAnalysis;
using Azure;
using Azure.Storage.Blobs.Specialized;
using Infrastructure.Services.Blob;
using Infrastructure.UnitTests.TestHelpers;
using NUnit.Framework;

namespace Infrastructure.UnitTests.Services.Blob;

[ExcludeFromCodeCoverage]
[TestFixture]
public class BlobLeaseManagerTests
{
    private BlobLeaseManager _blobLeaseManager;
    private AzuriteInitializer _azuriteInitializer;
    
    [SetUp]
    public async Task SetUp()
    {
        _azuriteInitializer = new AzuriteInitializer();
        await _azuriteInitializer.CreateUnitTestContainer();
    }

    [Test]
    public async Task GetLeaseIdTest()
    {
        _blobLeaseManager = new BlobLeaseManager(AzuriteInitializer.DefaultConnectionString, AzuriteInitializer.DefaultContainer);
        await _azuriteInitializer.CreateSampleFile("sample1");
        
        var leaseId = await _blobLeaseManager.SetBlobName("sample1").GetLease(new CancellationToken());
        
        Assert.IsInstanceOf<Guid>(Guid.Parse(leaseId));
    }
    
    [Test]
    public async Task GetEmptyOnNonExistingFile()
    {
        _blobLeaseManager = new BlobLeaseManager(AzuriteInitializer.DefaultConnectionString, AzuriteInitializer.DefaultContainer);
        
        var leaseId = await _blobLeaseManager.SetBlobName("nonexistingfile").GetLease(new CancellationToken());
        
        Assert.True(leaseId.Length == 0);
    }
    
    [Test]
    public async Task ReleaseFile()
    {
        _blobLeaseManager = new BlobLeaseManager(AzuriteInitializer.DefaultConnectionString, AzuriteInitializer.DefaultContainer);
        var blob = await _azuriteInitializer.CreateSampleFile("sample2");

        string lease = await _blobLeaseManager.SetBlobName("sample2").GetLease(new CancellationToken());
        
        Assert.ThrowsAsync<RequestFailedException>(
            async () => await blob.GetBlobLeaseClient().AcquireAsync(TimeSpan.FromSeconds(15)),
            "There is already a lease present.");

        await _blobLeaseManager.SetBlobName("sample2").ReleaseLease(lease, new CancellationToken());

        Assert.DoesNotThrowAsync(async () => await blob.GetBlobLeaseClient().AcquireAsync(TimeSpan.FromSeconds(15)));
    }
    
    [Test]
    public async Task RenewRelease()
    {
        _blobLeaseManager = new BlobLeaseManager(AzuriteInitializer.DefaultConnectionString, AzuriteInitializer.DefaultContainer);
        var blob = await _azuriteInitializer.CreateSampleFile("sample3");

        string lease = await _blobLeaseManager.SetBlobName("sample3").GetLease(new CancellationToken());
        
        Assert.ThrowsAsync<RequestFailedException>(
            async () => await blob.GetBlobLeaseClient().AcquireAsync(TimeSpan.FromSeconds(15)),
            "There is already a lease present.");

        Assert.True(await _blobLeaseManager.RenewLeaseAsync(lease, new CancellationToken()));
        
        Assert.ThrowsAsync<RequestFailedException>(async () => await blob.GetBlobLeaseClient().AcquireAsync(TimeSpan.FromSeconds(15)));
    }
    
    [TearDown]
    public async Task TearDown()
    {
       await _azuriteInitializer.Kill();
    }
}