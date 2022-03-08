using Azure.Storage.Blobs.Models;

namespace Application.Common.Interfaces
{
    public interface IBlobManagerService
    {
        Task<int> AddAsync(Stream fileStream, string filename, string contentType,
            IDictionary<string, string> metadata, CancellationToken ct);
        Task<int> UpdateAsync(string filename, IDictionary<string, string> metadata, CancellationToken ct);
        Task<BlobDownloadResult> DownloadAsync(string filename, int? id);
        Task<int> PromoteBlobVersionAsync(string filename, int id, CancellationToken ct);
    }
}
