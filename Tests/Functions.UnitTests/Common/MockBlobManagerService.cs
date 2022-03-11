using Application.Common.Interfaces.Blob;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Functions.UnitTests.Common
{
    public class MockBlobManagerService : IBlobCreatorService, IBlobManagerService
    {
        private IDictionary<string, BlobDownloadResult> blobs;
        public MockBlobManagerService()
        {
            blobs = new Dictionary<string, BlobDownloadResult>();
        }

        public async Task<HttpStatusCode> AddAsync(Stream fileStream, string filename, string contentType, IDictionary<string, string> metadata, CancellationToken ct)
        {
            blobs.Add(filename, MakeFakeDownloadResult(fileStream, contentType, metadata));

            return HttpStatusCode.Created;
        }

        public async Task<HttpStatusCode> AddAsync(Stream fileStream, string filename, string contentType, CancellationToken ct)
        {
            blobs.Add(filename, MakeFakeDownloadResult(fileStream, contentType));

            return HttpStatusCode.Created;
        }

        public async Task<HttpStatusCode> DeleteBlobAsync(string filename, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<BlobDownloadResult> DownloadAsync(string filename, int? id = 0)
        {
            return blobs[filename];
        }

        public async Task<IEnumerable<BlobItem>> GetBlobsInfoByName(string prefix, string size, string blobName, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<HttpStatusCode> PromoteBlobVersionAsync(string filename, int id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<HttpStatusCode> UpdateAsync(string filename, IDictionary<string, string> metadata, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        private BlobDownloadResult MakeFakeDownloadResult(Stream fileStream, string contentType, IDictionary<string, string>? metadata = null)
        {
            using (var memStream = new MemoryStream())
            {
                fileStream.CopyTo(memStream);
                var bytes = new BinaryData(memStream.ToArray());
                var details = BlobsModelFactory.BlobDownloadDetails(BlobType.Block, memStream.Length, contentType, default, DateTimeOffset.Now, metadata, default, default, default, default, default, 0, DateTimeOffset.Now, default, default, default, default, CopyStatus.Success, LeaseDurationType.Infinite, LeaseState.Available, LeaseStatus.Unlocked, default, default, default, default, default, default, default, default, default, default, default);

                return BlobsModelFactory.BlobDownloadResult(bytes, details);
            }
        }
    }
}
