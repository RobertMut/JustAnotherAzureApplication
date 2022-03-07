using Application.Common.Interfaces;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;


namespace Infrastructure.Services
{
    public class BlobManagerService : IBlobManagerService
    {
        private readonly BlobServiceClient _service;
        private readonly string _containerName;

        public BlobManagerService(string connectionString, string container)
        {
            _service = new BlobServiceClient(connectionString);
            _containerName = container;
        }
        public async Task<int> AddAsync(Stream fileStream, string filename, string contentType,
            IDictionary<string, string> metadata, CancellationToken ct)
        {
            var blobClient = _service.GetBlobContainerClient(_containerName).GetBlobClient(filename);
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
            var response = await _service.GetBlobContainerClient(_containerName).GetBlobClient(filename)
                .UploadAsync(fileStream, new BlobUploadOptions()
                {
                    HttpHeaders = new BlobHttpHeaders()
                    {
                        ContentType = contentType,
                    },

                }, ct);
            return response.GetRawResponse().Status;
        }
        public async Task<BlobDownloadResult> DownloadAsync(string filename)
        {

            var file = await _service.GetBlobContainerClient(_containerName).GetBlobClient(filename).DownloadContentAsync();

            return file;

        }
    }
}
