using Application.Common.Interfaces.Blob;
using Azure.Storage.Blobs.Models;
using Domain.Constants.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace API.IntegrationTests.Common
{
    public class MockBlobManagerService : IBlobManagerService
    {
        private IDictionary<string, BlobDownloadResult[]> _blobs;
        public static int FileLenght;
        public MockBlobManagerService()
        {

            var bytes = new byte[] { 00, 50, 00, 00, 40, 00, 03, 00, 00, 00, 00, 10 };
            _blobs = new Dictionary<string, BlobDownloadResult[]>()
            {
                {$"original_{Utils.DefaultId}_sample1.png", new [] {Utils.MakeFakeDownloadResult(new MemoryStream(bytes), $"original_{Utils.DefaultId}_sample1.png", "image/png", null, "0"),
                Utils.MakeFakeDownloadResult(new MemoryStream(bytes.Reverse().ToArray()),
                                        $"original_{Utils.DefaultId}_sample1.png",
                                        "image/png", null, "1")} },
                {$"original_{Utils.DefaultId}_sample2.png", new [] {Utils.MakeFakeDownloadResult(new MemoryStream(bytes), $"original_{Utils.DefaultId}_sample2.png", "image/png", null, "0"),
                Utils.MakeFakeDownloadResult(new MemoryStream(bytes.Reverse().ToArray()),
                                        $"original_{Utils.DefaultId}_sample2.png",
                                        "image/png", null, "1")} },
                {$"miniature_300x300_{Utils.DefaultId}_sample1.jpeg", new [] {Utils.MakeFakeDownloadResult(new MemoryStream(bytes), $"miniature_300x300_{Utils.DefaultId}_sample1.jpeg", "image/jpeg") }},
                {$"miniature_200x200_{Utils.DefaultId}_sample1.jpeg", new [] {Utils.MakeFakeDownloadResult(new MemoryStream(bytes), $"miniature_200x200_{Utils.DefaultId}_sample1.jpeg", "image/jpeg") }},
                {$"miniature_400x400_{Utils.DefaultId}_sample2.jpeg", new [] {Utils.MakeFakeDownloadResult(new MemoryStream(bytes), $"miniature_400x400_{Utils.DefaultId}_sample2.jpeg", "image/jpeg") }}
            };
        }

        public async Task<HttpStatusCode> AddAsync(Stream fileStream, string filename, string contentType, IDictionary<string, string> metadata, CancellationToken ct)
        {
            string splitFilename = filename.Split("-")[1];
            string miniatureName = $"{Prefixes.MiniatureImage}{metadata[Metadata.TargetWidth]}x{metadata[Metadata.TargetHeight]}-{splitFilename}";

            AddNewBlobOrPrepend(fileStream, filename, contentType, metadata);
            AddNewBlobOrPrepend(fileStream, miniatureName, contentType);

            return HttpStatusCode.Created;
        }

        public async Task<HttpStatusCode> AddAsync(Stream fileStream, string filename, string contentType, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<BlobDownloadResult> DownloadAsync(string filename, int? id = 0)
        {
            return _blobs[filename][id.GetValueOrDefault()];

        }

        public async Task<HttpStatusCode> UpdateAsync(string filename, IDictionary<string, string> metadata, CancellationToken ct)
        {
            var blob = _blobs[filename][0];
            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename.Split("-")[1]);
            string miniatureName = $"{Prefixes.MiniatureImage}{metadata[Metadata.TargetWidth]}x{metadata[Metadata.TargetHeight]}-{filenameWithoutExtension}.{metadata[Metadata.TargetType]}";

            AddNewBlobOrPrepend(blob.Content.ToStream(), miniatureName, $"image/{metadata[Metadata.TargetType]}");

            return HttpStatusCode.OK;
        }

        public async Task<HttpStatusCode> PromoteBlobVersionAsync(string filename, int id, CancellationToken ct)
        {
            var blob = _blobs[filename];

            _blobs[filename] = blob.Prepend(blob[id]).ToArray();

            return HttpStatusCode.Created;
        }

        private void AddNewBlobOrPrepend(Stream fileStream, string filename, string contentType, IDictionary<string, string> metadata = null)
        {

            if (_blobs.ContainsKey(filename))
            {
                var blob = Utils.MakeFakeDownloadResult(fileStream, filename, contentType, metadata);
                var blobs = _blobs[filename];

                _blobs[filename] = blobs.Prepend(blob).ToArray();
            }
            else
            {
                _blobs.Add(filename, new[] { Utils.MakeFakeDownloadResult(fileStream, filename, contentType, metadata) });
            }
        }

        public async Task<HttpStatusCode> DeleteBlobAsync(string filename, CancellationToken ct)
        {
            _blobs.Remove(filename);

            return HttpStatusCode.Accepted;
        }

        public async Task<IEnumerable<BlobItem>> GetBlobsInfoByName(string prefix, string size, string blobName, string userId, CancellationToken ct)
        {
            var blobsNames = _blobs.Keys.Where(name => name.Contains(prefix))
                .Where(x => x.Contains($"{Path.GetFileNameWithoutExtension(blobName)}."))
                .Where(x => x.Contains(size))
                .Where(x => x.Contains(userId))
                .ToList();
            var blobs = new List<BlobItem>();
            foreach (var blob in blobsNames)
            {
                blobs.Add(BlobsModelFactory.BlobItem(blob, default, Utils.MakeFakeBlobProperties(_blobs[blob][0].Details.ContentType, _blobs[blob][0].Details.ContentEncoding),
                    null, _blobs[blob][0].Details.VersionId, null, _blobs[blob][0].Details.Metadata, null, null, null));
            }

            return blobs.AsEnumerable();
        }
    }
}
