using Application.Common.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Infrastructure.Services
{
    public class BlobCreatorService : IBlobCreatorService
    {
        private readonly BlobContainerClient _client;

        public BlobCreatorService(string connectionString, string container)
        {
            _client = new BlobServiceClient(connectionString).GetBlobContainerClient(container);
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
    }
}
