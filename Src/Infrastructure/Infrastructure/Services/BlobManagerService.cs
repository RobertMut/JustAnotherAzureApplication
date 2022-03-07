using Application.Common.Interfaces;
using System.Linq;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Infrastructure.Services
{
    public class BlobManagerService : IBlobManagerService
    {
        private readonly BlobContainerClient _client;

        public BlobManagerService(string connectionString, string container)
        {
            _client = new BlobServiceClient(connectionString).GetBlobContainerClient(container);
        }
        public async Task<int> AddAsync(Stream fileStream, string filename, string contentType,
            IDictionary<string, string> metadata, CancellationToken ct)
        {
            var blobClient = _client.GetBlobClient(filename);
            var blobOptions = new BlobUploadOptions()
            {
                Metadata = metadata,
                HttpHeaders = new BlobHttpHeaders()
                {
                    ContentType = contentType
                },
            };

            var response = await blobClient.UploadAsync(fileStream, blobOptions, ct);
            return response.GetRawResponse().Status;
        }
        public async Task<int> AddAsync(Stream fileStream, string filename, string contentType, CancellationToken ct)
        {

            var response = await _client.GetBlobClient(filename)
                .UploadAsync(fileStream, new BlobUploadOptions()
                {
                    HttpHeaders = new BlobHttpHeaders()
                    {
                        ContentType = contentType,
                    },

                }, ct);
            return response.GetRawResponse().Status;
        }
        public async Task<BlobDownloadResult> DownloadAsync(string filename, int? id)
        {
            var client = _client.GetBlobClient(filename);
            if (id == null) return await client.DownloadContentAsync();
            var versions = await GetBlobVersions(filename);
            return await client.WithVersion(versions[id.Value].VersionId).DownloadContentAsync();

        }

        public async Task<int> PromoteBlobVersionAsync(string filename, int id, CancellationToken ct)
        {
            var client = _client.GetBlobClient($"original-{filename}");
            var versions = await GetBlobVersions($"original-{filename}");
            var sourceBlobUri = new Uri($"{_client.Uri.AbsoluteUri}/{_client.Name}/original-{filename}?versionId={versions[id].VersionId}");
            var operation = await client.StartCopyFromUriAsync(sourceBlobUri, null, ct); //crashes
            return operation.GetRawResponse().Status;

        }

        public async Task<int> UpdateAsync(string filename, IDictionary<string, string> metadata, CancellationToken ct)
        {
            var client = _client.GetBlobClient($"original-{filename}");
            var response = await client.SetMetadataAsync(metadata, default, ct);
            return response.GetRawResponse().Status;

        }

        private async Task<BlobItem[]> GetBlobVersions(string filename)
        {
            return _client.GetBlobs(BlobTraits.All, BlobStates.All).Where(x => x.Name.StartsWith(filename)).Reverse().ToArray();
        }
    }
}
