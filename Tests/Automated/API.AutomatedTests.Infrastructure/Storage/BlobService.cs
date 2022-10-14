using API.AutomatedTests.Implementation.Common.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;

namespace API.AutomatedTests.Infrastructure.Storage;

public class BlobService : IBlobService
{
    private readonly BlobContainerClient blobContainerClient;

    public BlobService(IConfiguration configuration)
    {
        string connectionString = configuration["AzureWebJobsStorage"];
        blobContainerClient = new BlobServiceClient(connectionString).GetBlobContainerClient("ImagesContainer");
    }

    public async Task<bool> CheckIfBlobExists(string blobName,
        CancellationToken ct) =>
        await blobContainerClient.GetBlobClient(blobName).ExistsAsync(ct);

    public async Task<BlobContentInfo> AddBlobWithMetadata(string blobName,
        byte[] content,
        CancellationToken ct,
        Dictionary<string, string> metadata = null) =>
        (await blobContainerClient.GetBlobClient(blobName).UploadAsync(BinaryData.FromBytes(content), new BlobUploadOptions
        {
            Metadata = metadata
        }, ct)).Value;

    public async Task DeleteBlobIfExists(string blobName, 
        CancellationToken ct) =>
        await blobContainerClient.GetBlobClient(blobName).DeleteIfExistsAsync(cancellationToken: ct);
}