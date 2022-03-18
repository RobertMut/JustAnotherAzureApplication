using Azure.Storage.Blobs.Models;
using System.Net;

namespace Application.Common.Interfaces.Blob
{
    public interface IBlobManagerService
    {
        Task<HttpStatusCode> AddAsync(Stream fileStream, string filename, string contentType,
            IDictionary<string, string> metadata, CancellationToken ct);

        Task<HttpStatusCode> UpdateAsync(string filename, IDictionary<string, string> metadata = null, CancellationToken ct = default);

        Task<BlobDownloadResult> DownloadAsync(string filename, int? id);

        Task<HttpStatusCode> PromoteBlobVersionAsync(string filename, int id, CancellationToken ct);

        Task<HttpStatusCode> DeleteBlobAsync(string filename, CancellationToken ct);

        Task<IEnumerable<BlobItem>> GetBlobsInfoByName(string prefix, string size, string blobName, string userId, CancellationToken ct);
    }
}
