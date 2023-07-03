using Application.Common.Interfaces.Blob;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using System.Net;

namespace Infrastructure.Services.Blob;

public class BlobLeaseManager : IBlobLeaseManager
{
    private readonly BlobContainerClient _blobContainerClient;
    private PageBlobClient _pageBlobClient;
    
    public BlobLeaseManager(string connectionString, string container)
    {
        var blobClientOptions = new BlobClientOptions();
        blobClientOptions.Retry.Delay = TimeSpan.FromSeconds(5);
        blobClientOptions.Retry.MaxRetries = 3;

        _blobContainerClient = new BlobServiceClient(connectionString, blobClientOptions).GetBlobContainerClient(container);
    }

    /// <summary>
    /// Get current lease
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Lease id</returns>
    /// <exception cref="RequestFailedException">When error occured while getting lease</exception>
    public async Task<string> GetLease(CancellationToken cancellationToken)
    {
        try
        {
            var leaseClient = _pageBlobClient.GetBlobLeaseClient();
            var lease = await leaseClient.AcquireAsync(TimeSpan.FromSeconds(15), null, cancellationToken);

            return lease.Value.LeaseId;
        }
        catch (RequestFailedException storageException)
        {
            if (storageException.Status == (int)HttpStatusCode.NotFound)
            {
                return string.Empty;
            }
            else
            {
                throw storageException;
            }
        }
    }

    /// <summary>
    /// Releases lease
    /// </summary>
    /// <param name="leaseId">Lease id</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    public async Task ReleaseLease(string leaseId, CancellationToken cancellationToken)
    {
        var leaseClient = _pageBlobClient.GetBlobLeaseClient(leaseId);

        await leaseClient.ReleaseAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Renews lease
    /// </summary>
    /// <param name="leaseId">Current lease id</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    public async Task<bool> RenewLeaseAsync(string leaseId, CancellationToken cancellationToken)
    {
        var leaseClient = _pageBlobClient.GetBlobLeaseClient(leaseId);

        await leaseClient.RenewAsync(cancellationToken: cancellationToken);

        return true;
    }

    /// <summary>
    /// Set blob client to specific blob name
    /// </summary>
    /// <param name="blobName">Blob name</param>
    /// <returns><see cref="IBlobLeaseManager"/></returns>
    public IBlobLeaseManager SetBlobName(string blobName)
    {
        _pageBlobClient = _blobContainerClient.GetPageBlobClient(blobName);

        return this;
    }
}