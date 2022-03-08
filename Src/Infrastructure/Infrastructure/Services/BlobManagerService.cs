﻿using Application.Common.Interfaces;
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
        /// <summary>
        /// Promotes previous blob version to actual.
        /// Opens stream with blob and uploads it again due to lack of method to promote in a simple way.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns>
        /// A int representing HTTP status of operation.
        /// </returns>
        public async Task<int> PromoteBlobVersionAsync(string filename, int id, CancellationToken ct)
        {
            var client = _client.GetBlobClient($"original-{filename}");
            var version = await GetBlobVersions($"original-{filename}");
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
                return response.GetRawResponse().Status;
            }

        }

        public async Task<int> UpdateAsync(string filename, IDictionary<string, string> metadata, CancellationToken ct)
        {
            var client = _client.GetBlobClient($"original-{filename}");
            var response = await client.SetMetadataAsync(metadata, default, ct);
            return response.GetRawResponse().Status;

        }
        /// <summary>
        /// Gets all blobs (and their versions) using provided filename.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>
        /// BlobItem array with basic data about blob
        /// </returns>
        private async Task<BlobItem[]> GetBlobVersions(string filename)
        {
            return _client.GetBlobs(BlobTraits.All, BlobStates.All).Where(x => x.Name.StartsWith(filename)).Reverse().ToArray();
        }
    }
}
