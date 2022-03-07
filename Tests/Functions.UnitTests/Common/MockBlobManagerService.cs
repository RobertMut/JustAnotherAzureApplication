using Application.Common.Interfaces;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Functions.UnitTests.Common
{
    public class MockBlobManagerService : IBlobManagerService
    {
        private IDictionary<string, BlobDownloadResult> blobs;
        public MockBlobManagerService()
        {
            blobs = new Dictionary<string, BlobDownloadResult>();
        }

        public async Task<int> AddAsync(Stream fileStream, string filename, string contentType, IDictionary<string, string> metadata, CancellationToken ct)
        {
            blobs.Add($"original-{filename}", MakeFakeDownloadResult(fileStream, contentType, metadata));
            return 201;
        }

        public async Task<int> AddAsync(Stream fileStream, string filename, string contentType, CancellationToken ct)
        {
            blobs.Add(filename, MakeFakeDownloadResult(fileStream, contentType));
            return 201;
        }

        public async Task<BlobDownloadResult> DownloadAsync(string filename)
        {
            return blobs[filename];
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
