namespace Application.Common.Interfaces.Blob;

/// <summary>
/// Determines IBlobLeaseManager interface to manage blob leases in container
/// </summary>
public interface IBlobLeaseManager
{
    /// <summary>
    /// Releases lease
    /// </summary>
    /// <param name="leaseId">Lease id</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns></returns>
    Task ReleaseLease(string leaseId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets lease
    /// </summary>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>Lease id</returns>
    Task<string> GetLease(CancellationToken cancellationToken);

    /// <summary>
    /// Renews lease
    /// </summary>
    /// <param name="leaseId">LeaseId</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>True</returns>
    Task<bool> RenewLeaseAsync(string leaseId, CancellationToken cancellationToken);

    /// <summary>
    /// Sets blob client to specific blob name
    /// </summary>
    /// <param name="blobName">Blob name</param>
    /// <returns>This</returns>
    IBlobLeaseManager SetBlobName(string blobName);
}