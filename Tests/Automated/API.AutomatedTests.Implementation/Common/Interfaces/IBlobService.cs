using Azure.Storage.Blobs.Models;

namespace API.AutomatedTests.Implementation.Common.Interfaces;

public interface IBlobService
{
    Task<bool> CheckIfBlobExists(string blobName,
        CancellationToken ct);

    Task<BlobContentInfo> AddBlobWithMetadata(string blobName,
        byte[] content,
        CancellationToken ct,
        Dictionary<string, string> metadata = null);

    Task DeleteBlobIfExists(string blobName, 
        CancellationToken ct);
}