using Application.Common.Interfaces;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Functions.UnitTests.Common
{
    public class MockBlobManagerService : IBlobManagerService
    {
        public IDictionary<string, BlobDownloadResult> Blobs;
        public MockBlobManagerService()
        {
            Blobs = new Dictionary<string, BlobDownloadResult>();
        }

        public async Task<int> AddAsync(Stream fileStream, string filename, string contentType, IDictionary<string, string> metadata, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<int> AddAsync(Stream fileStream, string filename, string contentType, CancellationToken ct)
        {
            Blobs.Add($"miniature-{filename}", MakeFakeDownloadResult(fileStream, filename, contentType));
            return 201;
        }

        public async Task<BlobDownloadResult> DownloadAsync(string filename)
        {
            throw new NotImplementedException();
        }
        private BlobDownloadResult MakeFakeDownloadResult(Stream fileStream, string filename, string contentType, IDictionary<string, string>? metadata = null)
        {
            using (var memStream = new MemoryStream())
            {
                fileStream.CopyTo(memStream);
                var bytes = new BinaryData(memStream.ToArray());
                var details = BlobsModelFactory.BlobDownloadDetails(BlobType.Block, memStream.Length, contentType, new byte[] { 00 }, DateTimeOffset.Now, metadata, null, null, null, null, null, 1, DateTimeOffset.Now, null, null, null, null, CopyStatus.Success, LeaseDurationType.Infinite, LeaseState.Available, LeaseStatus.Unlocked, null, 1, false, null, null, new byte[] { 00 }, 0, null, false, null, null);
                return BlobsModelFactory.BlobDownloadResult(bytes, details);
            }
        }
    }
}
