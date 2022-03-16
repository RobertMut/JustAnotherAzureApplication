namespace Application.Common.Interfaces.Blob
{
    public interface IBlobLeaseManager
    {
        Task ReleaseLease(string leaseId, CancellationToken cancellationToken);

        Task<string> GetLease(CancellationToken cancellationToken);

        Task<bool> RenewLeaseAsync(string leaseId, CancellationToken cancellationToken);

        IBlobLeaseManager SetBlobName(string blobName);
    }
}
