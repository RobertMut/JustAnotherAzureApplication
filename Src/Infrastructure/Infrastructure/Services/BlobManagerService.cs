using Application.Common.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Infrastructure.Services
{
    public class BlobManagerService : IBlobManagerService
    {
        readonly BlobContainerClient _client;
        public BlobManagerService()
        {
            //l1XveR1poEtoV9WEeXbuzYqPdLQyThEXVrCh8tWx8Fp4n5qfK/9rG9cnD2DzzlifQsu7/kNvDw+Z+AStz2PSMw==
            //jaaastorage
            BlobServiceClient service = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=jaaastorage;AccountKey=l1XveR1poEtoV9WEeXbuzYqPdLQyThEXVrCh8tWx8Fp4n5qfK/9rG9cnD2DzzlifQsu7/kNvDw+Z+AStz2PSMw==;EndpointSuffix=core.windows.net");
            _client = service.GetBlobContainerClient("jaaablob");

            //
        }
        public async Task<int> AddAsync(Stream fileStream, string filename, string contentType, CancellationToken ct)
        {
            var response = await _client.GetBlobClient(filename).UploadAsync(fileStream, new BlobUploadOptions()
            {
                HttpHeaders = new BlobHttpHeaders()
                {
                    ContentType = contentType
                }
            }, ct);
            return response.GetRawResponse().Status;
        }
        //public async Task DownloadAsync(string filename, CancellationToken ct)
        //{

        //}
    }
}
