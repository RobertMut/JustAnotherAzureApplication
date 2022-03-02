using Azure.Storage.Blobs.Models;

namespace Application.Common.Interfaces
{
    public interface IBlobManagerService
    {
        Task<int> AddAsync(Stream fileStream, string filename, string contentType, string container, CancellationToken ct);
        Task<BlobDownloadResult> DownloadAsync(string filename, string container);
    }
}
