using Application.Common.Interfaces.Blob;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Net;

namespace Infrastructure.Services
{
    public class BlobCreatorService : IBlobCreatorService
    {
        private readonly BlobContainerClient _client;

        public BlobCreatorService(string connectionString, string container)
        {
            _client = new BlobServiceClient(connectionString).GetBlobContainerClient(container);
        }

        public async Task<HttpStatusCode> AddAsync(Stream fileStream, string filename, string contentType, CancellationToken ct)
        {
            var response = await _client.GetBlobClient(filename)
                .UploadAsync(fileStream, new BlobUploadOptions()
                {
                    HttpHeaders = new BlobHttpHeaders()
                    {
                        ContentType = contentType,
                    },

                }, ct);

            return (HttpStatusCode)response.GetRawResponse().Status;
        }
    }
}
