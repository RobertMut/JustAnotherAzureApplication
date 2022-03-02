using Application.Common.Interfaces;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Infrastructure.Services
{
    public class BlobManagerService : IBlobManagerService
    {
        private readonly BlobServiceClient _service;

        public BlobManagerService(string connectionString)
        {
            _service = new BlobServiceClient(connectionString);
        }
        public async Task<int> AddAsync(Stream fileStream, string filename, string contentType, string container, CancellationToken ct)
        {
            var response = await _service.GetBlobContainerClient(container).GetBlobClient(filename).UploadAsync(fileStream, new BlobUploadOptions()
            {
                HttpHeaders = new BlobHttpHeaders()
                {
                    ContentType = contentType
                }
            }, ct);
            return response.GetRawResponse().Status;
        }
        public async Task<BlobDownloadResult> DownloadAsync(string filename, string container)
        {

            var file = await _service.GetBlobContainerClient(container).GetBlobClient(filename).DownloadContentAsync();

            return file;

        }
    }
}
