using Application.Common.Interfaces;
using Azure.Storage.Blobs;
using Microsoft.WindowsAzure.Storage.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task AddAsync(Stream fileStream, CancellationToken ct)
        {
            await _client.GetBlobClient("").UploadAsync(fileStream, ct);
        }
    }
}
