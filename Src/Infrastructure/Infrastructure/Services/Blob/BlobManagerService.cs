using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Application.Common.Interfaces.Blob;
using System.Net;

namespace Infrastructure.Services.Blob
{
    /// <summary>
    /// Class BlobManagerService
    /// </summary>
    public class BlobManagerService : IBlobManagerService
    {
        private readonly BlobContainerClient _blobContainerClient;

        /// <summary>
        /// Initializes new instance of <see cref="BlobManagerService" /> class.
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="container">Container</param>
        public BlobManagerService(string connectionString, string container)
        {
            _blobContainerClient = new BlobServiceClient(connectionString).GetBlobContainerClient(container);

            if (!_blobContainerClient.Exists())
            {
                _blobContainerClient.Create(PublicAccessType.BlobContainer);
            }
        }

        /// <summary>
        /// Adds blob
        /// </summary>
        /// <param name="fileStream">File stream</param>
        /// <param name="filename">Filename</param>
        /// <param name="contentType">File content type</param>
        /// <param name="metadata">Metadata</param>
        /// <param name="ct"><see cref="CancellationToken"/></param>
        /// <returns>Http status code from blob</returns>
        public async Task<HttpStatusCode> AddAsync(Stream fileStream, string filename, string contentType,
            IDictionary<string, string> metadata = null, CancellationToken ct = default)
        {
            var blobClient = _blobContainerClient.GetBlobClient(filename);
            var blobOptions = new BlobUploadOptions()
            {
                Metadata = metadata,
                HttpHeaders = new BlobHttpHeaders()
                {
                    ContentType = contentType
                },
            };
            var response = await blobClient.UploadAsync(fileStream, blobOptions, ct);

            return (HttpStatusCode)response.GetRawResponse().Status;
        }

        /// <summary>
        /// Deletes blob from container
        /// </summary>
        /// <param name="filename">Blob name to delete</param>
        /// <param name="ct"><see cref="CancellationToken"/></param>
        /// <returns>Http status code of operation</returns>
        public async Task<HttpStatusCode> DeleteBlobAsync(string filename, CancellationToken ct)
        {
            var response = await _blobContainerClient.DeleteBlobAsync(filename, DeleteSnapshotsOption.None, null, ct);

            return (HttpStatusCode)response.Status;
        }

        /// <summary>
        /// Download specific blob
        /// </summary>
        /// <param name="filename">Blob name</param>
        /// <param name="id">Version id</param>
        /// <returns>Blob download result</returns>
        public async Task<BlobDownloadResult> DownloadAsync(string filename, int? id)
        {
            var client = _blobContainerClient.GetBlobClient(filename);
            if (!id.HasValue)
            {
                return await client.DownloadContentAsync();
            }
            var versions = await GetBlobVersions(filename);

            return await client.WithVersion(versions[id.Value].VersionId).DownloadContentAsync();
        }

        /// <summary>
        /// Gets blob info list
        /// </summary>
        /// <param name="prefix">Blob name prefix</param>
        /// <param name="size">Blob size</param>
        /// <param name="blobName">Filename</param>
        /// <param name="userId">User guid</param>
        /// <param name="ct"><see cref="CancellationToken"/></param>
        /// <returns>list of blob item</returns>
        public async Task<IEnumerable<BlobItem>> GetBlobsInfoByName(string prefix, string size, string blobName, string userId, CancellationToken ct)
        {
            return _blobContainerClient.GetBlobs(BlobTraits.All, BlobStates.None, prefix, ct)
                .Where(x => x.Name.Contains($"_{Path.GetFileNameWithoutExtension(blobName)}."))
                .Where(x => x.Name.Contains(string.IsNullOrWhiteSpace(size) ? string.Empty : $"_{size}_"))
                .Where(x => x.Name.Contains(userId)).ToList();
        }

        /// <summary>
        /// Promotes previous blob version to actual.
        /// Opens stream with blob and uploads it again due to lack of method to promote in a simple way.
        /// </summary>
        /// <param name="filename">Blob name</param>
        /// <param name="id">Version Id</param>
        /// <param name="ct"><see cref="CancellationToken"/></param>
        /// <returns>
        /// An int representing HTTP status of operation.
        /// </returns>
        public async Task<HttpStatusCode> PromoteBlobVersionAsync(string filename, int id, CancellationToken ct)
        {
            var blobClient = _blobContainerClient.GetBlobClient(filename);
            var version = await GetBlobVersions(filename);
            var properties = blobClient.GetProperties().Value;
            using (var stream = await blobClient.WithVersion(version[id].VersionId).OpenReadAsync(new BlobOpenReadOptions(false), ct))
            {
                var response = await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    Metadata = properties.Metadata,
                    HttpHeaders = new BlobHttpHeaders()
                    {
                        ContentType = properties.ContentType,
                        CacheControl = properties.CacheControl,
                    },
                    AccessTier = properties.AccessTier
                }, ct);

                return (HttpStatusCode)response.GetRawResponse().Status;
            }
        }

        /// <summary>
        /// Updates blob
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="metadata">Metadata</param>
        /// <param name="ct"><see cref="CancellationToken"/></param>
        /// <returns>Http Code of operation</returns>
        public async Task<HttpStatusCode> UpdateAsync(string filename, IDictionary<string, string> metadata, CancellationToken ct)
        {
            var client = _blobContainerClient.GetBlobClient(filename);
            var response = await client.SetMetadataAsync(metadata, default, ct);

            return (HttpStatusCode)response.GetRawResponse().Status;
        }
        
        /// <summary>
        /// Gets all blobs (and their versions) using provided filename.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>
        /// A BlobItem array with basic data about blob
        /// </returns>
        private async Task<BlobItem[]> GetBlobVersions(string filename)
        {
            return _blobContainerClient.GetBlobs(BlobTraits.All, BlobStates.All).Where(x => x.Name.StartsWith(filename)).Reverse().ToArray();
        }
    }
}
