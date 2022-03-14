using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Application.Common.Interfaces.Blob;
using System.Net;

namespace Infrastructure.Services
{
    public class BlobManagerService : IBlobManagerService 
    {
        private readonly BlobContainerClient _blobContainerClient;

        public BlobManagerService(string connectionString, string container)
        {
            _blobContainerClient = new BlobServiceClient(connectionString).GetBlobContainerClient(container);
        }

        public async Task<HttpStatusCode> AddAsync(Stream fileStream, string filename, string contentType,
            IDictionary<string, string> metadata, CancellationToken ct)
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

        public async Task<HttpStatusCode> DeleteBlobAsync(string filename, CancellationToken ct)
        {
            var response = await _blobContainerClient.DeleteBlobAsync(filename, DeleteSnapshotsOption.None, null, ct);

            return (HttpStatusCode)response.Status;
        }

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

        public async Task<IEnumerable<BlobItem>> GetBlobsInfoByName(string prefix, string size, string blobName, CancellationToken ct)
        {
            return _blobContainerClient.GetBlobs(BlobTraits.All, BlobStates.None, prefix, ct)
                .Where(x => x.Name.Contains($"{Path.GetFileNameWithoutExtension(blobName)}."))
                .Where(x => x.Name.Contains(size))
                .ToList();
        }

        /// <summary>
        /// Promotes previous blob version to actual.
        /// Opens stream with blob and uploads it again due to lack of method to promote in a simple way.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns>
        /// An int representing HTTP status of operation.
        /// </returns>
        public async Task<HttpStatusCode> PromoteBlobVersionAsync(string filename, int id, CancellationToken ct)
        {
            var client = _blobContainerClient.GetBlobClient(filename);
            var version = await GetBlobVersions(filename);
            var properties = client.GetProperties().Value;
            using (var stream = await client.WithVersion(version[id].VersionId).OpenReadAsync(new BlobOpenReadOptions(false), ct))
            {
                var response = await client.UploadAsync(stream, new BlobUploadOptions
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
