using Azure.Storage.Blobs.Models;
using System.Net;

namespace Application.Common.Interfaces.Blob
{
    /// <summary>
    /// Determines IBlobManagerService to manage blobs inside blob container
    /// </summary>
    public interface IBlobManagerService
    {
        /// <summary>
        /// Adds blob
        /// </summary>
        /// <param name="fileStream">Stream with file data</param>
        /// <param name="filename">Filename to be saved</param>
        /// <param name="contentType">File content type</param>
        /// <param name="metadata">Metadatas</param>
        /// <param name="ct">CancellationToken</param>
        /// <returns>HttpStatusCode of operation</returns>
        Task<HttpStatusCode> AddAsync(Stream fileStream, string filename, string contentType,
            IDictionary<string, string> metadata, CancellationToken ct);
        
        /// <summary>
        /// Updates blob
        /// </summary>
        /// <param name="filename">Filename to be updated</param>
        /// <param name="metadata">Metadatas</param>
        /// <param name="ct">CancellationToken</param>
        /// <returns>HttpStatusCode of operation</returns>
        Task<HttpStatusCode> UpdateAsync(string filename, IDictionary<string, string> metadata = null, CancellationToken ct = default);

        /// <summary>
        /// Downloads blob with version id
        /// </summary>
        /// <param name="filename">Blob name</param>
        /// <param name="id">Version id</param>
        /// <returns>
        /// <see cref="BlobDownloadResult"/>
        /// </returns>
        Task<BlobDownloadResult> DownloadAsync(string filename, int? id);

        /// <summary>
        /// Promotes blob version to newest
        /// </summary>
        /// <param name="filename">Blob name</param>
        /// <param name="id">Version id to promote</param>
        /// <param name="ct">CancellationToken</param>
        /// <returns>HttpStatusCode of operation</returns>
        Task<HttpStatusCode> PromoteBlobVersionAsync(string filename, int id, CancellationToken ct);

        /// <summary>
        /// Deletes blob
        /// </summary>
        /// <param name="filename">Blob name</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>HttpStatusCode of operation</returns>
        Task<HttpStatusCode> DeleteBlobAsync(string filename, CancellationToken ct);

        /// <summary>
        /// Gets info for blobs related with image
        /// </summary>
        /// <param name="prefix">Blob prefix name</param>
        /// <param name="size">Image size</param>
        /// <param name="blobName">Filename</param>
        /// <param name="userId">User guid</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>HttpStatusCode of operation</returns>
        Task<IEnumerable<BlobItem>> GetBlobsInfoByName(string prefix, string size, string blobName, string userId, CancellationToken ct);
    }
}
