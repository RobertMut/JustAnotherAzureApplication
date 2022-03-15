using Application.Common.Interfaces.Blob;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using System.Net;

namespace Infrastructure.Services
{
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

        public async Task<string> GetLease(CancellationToken cancellationToken)
        {
            try
            {
                var leaseClient = _pageBlobClient.GetBlobLeaseClient();
                var lease = await leaseClient.AcquireAsync(TimeSpan.FromSeconds(15), null, cancellationToken);

                return lease.Value.LeaseId;
            } catch (RequestFailedException storageException)
            {
                if(storageException.Status == (int) HttpStatusCode.NotFound)
                {
                    return string.Empty;
                } else
                {
                    throw storageException;
                }
            }
        }

        public async Task ReleaseLease(string leaseId, CancellationToken cancellationToken)
        {
            var leaseClient = _pageBlobClient.GetBlobLeaseClient(leaseId);

            await leaseClient.ReleaseAsync(cancellationToken: cancellationToken);
        }

        public async Task<bool> RenewLeaseAsync(string leaseId, CancellationToken cancellationToken)
        {
            var leaseClient = _pageBlobClient.GetBlobLeaseClient(leaseId);

            await leaseClient.RenewAsync(cancellationToken: cancellationToken);

            return true;
        }

        public IBlobLeaseManager SetBlobName(string blobName)
        {
            _pageBlobClient = _blobContainerClient.GetPageBlobClient(blobName);

            return this;
        }
    }
}
