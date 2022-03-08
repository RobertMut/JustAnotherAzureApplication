using Application.Common.Interfaces;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.IntegrationTests.Common
{
    public class MockBlobManagerService : IBlobManagerService
    {
        private IDictionary<string, BlobDownloadResult[]> blobs;
        public MockBlobManagerService()
        {

            var bytes = new byte[] { 00, 50, 00, 00, 40, 00, 03, 00, 00, 00, 00, 10 };
            blobs = new Dictionary<string, BlobDownloadResult[]>()
            {
                {"original-sample.png", new [] {MakeFakeDownloadResult(new MemoryStream(bytes), "original-sample.png", "image/png"),
                MakeFakeDownloadResult(new MemoryStream(bytes.Reverse().ToArray()),
                                       "original-sample.png",
                                       "image/png")} },
                {"miniature-sample.jpeg", new [] {MakeFakeDownloadResult(new MemoryStream(bytes), "miniature-sample.jpeg", "image/jpeg") } }
            };
        }
        public async Task<int> AddAsync(Stream fileStream, string filename, string contentType, IDictionary<string, string> metadata, CancellationToken ct)
        {
            var fakeResult = MakeFakeDownloadResult(fileStream, filename, contentType, metadata);
            PrependOrAddToDictionary("miniature", filename, fakeResult);
            return 201;
        }

        public async Task<int> AddAsync(Stream fileStream, string filename, string contentType, CancellationToken ct)
        {
            var fakeResult = MakeFakeDownloadResult(fileStream, filename, contentType);
            PrependOrAddToDictionary("miniature", filename, fakeResult);
            return 201;
        }

        public async Task<BlobDownloadResult> DownloadAsync(string filename, int? id = 0)
        {
            return blobs[filename][id.GetValueOrDefault()];

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
        public async Task<int> UpdateAsync(string filename, IDictionary<string, string> metadata, CancellationToken ct)
        {
            string filenameWithPrefix = $"original-{filename}";
            var blob = blobs[filenameWithPrefix];
            using (var stream = new MemoryStream(blob[0].Content.ToArray()))
            {
                var updated = MakeFakeDownloadResult(stream, filenameWithPrefix, blob[0].Details.ContentType, metadata);
                blobs[filenameWithPrefix] = blob.Prepend(updated).ToArray();
                blobs[$"miniature-{filename}"] = blob.Prepend(updated).ToArray();
            }
            return 200;
        }

        public async Task<int> PromoteBlobVersionAsync(string filename, int id, CancellationToken ct)
        {
            string filenameWithPrefix = $"original-{filename}";
            var blob = blobs[filenameWithPrefix];
            (blob[id], blob[0]) = (blob[0], blob[id]);
            blobs[filenameWithPrefix] = blob;
            return 201;
        }
        private void PrependOrAddToDictionary(string filenamePrefix, string filename, BlobDownloadResult fakeResult)
        {
            var filenameWithPrefix = $"{filenamePrefix}-{filename}";
            if (blobs.ContainsKey(filename))
            {
                var blob = blobs[filenameWithPrefix];
                blobs[filenameWithPrefix] = blob.Prepend(fakeResult).ToArray();
            }
            else
                blobs.Add($"miniature-{filename}", new[] { fakeResult });
        }
    }
}
