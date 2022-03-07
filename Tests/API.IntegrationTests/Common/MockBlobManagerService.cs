using Application.Common.Interfaces;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace API.IntegrationTests.Common
{
    public class MockBlobManagerService : IBlobManagerService
    {
        private IDictionary<string, BlobDownloadResult> blobs;
        public static int FileLenght;
        public MockBlobManagerService()
        {

            var bytes = new byte[] { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 };
            FileLenght = bytes.Length;
            blobs = new Dictionary<string, BlobDownloadResult>()
            {
                {"miniature-sample.jpg", MakeFakeDownloadResult(new MemoryStream(bytes), "miniature-sample.jpg", "image/jpeg") }
            };
        }
        public async Task<int> AddAsync(Stream fileStream, string filename, string contentType, IDictionary<string, string> metadata, CancellationToken ct)
        {
            blobs.Add($"miniature-{filename}", MakeFakeDownloadResult(fileStream, filename, contentType, metadata));
            return 201;
        }

        public async Task<int> AddAsync(Stream fileStream, string filename, string contentType, CancellationToken ct)
        {
            blobs.Add($"miniature-{filename}", MakeFakeDownloadResult(fileStream, filename, contentType));
            return 201;
        }

        public async Task<BlobDownloadResult> DownloadAsync(string filename)
        {
            return blobs[filename];

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
