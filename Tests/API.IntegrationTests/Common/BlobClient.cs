using Azure.Storage.Blobs.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.IntegrationTests.Common
{
    public class BlobClient : Azure.Storage.Blobs.BlobClient
    {

        private string _version = "0";
        private string _blobName;

        public BlobClient()
        {
            var bytes = new byte[] { 00, 50, 00, 00, 40, 00, 03, 00, 00, 00, 00, 10 };
            Utils.Repository = new Dictionary<string, BlobDownloadResult[]>()
            {
                {"original-sample.png", new [] {Utils.MakeFakeDownloadResult(new MemoryStream(bytes), "original-sample.png", "image/png", null, "0"),
                Utils.MakeFakeDownloadResult(new MemoryStream(bytes.Reverse().ToArray()),
                                        "original-sample.png",
                                        "image/png", null, "1")} },
                {"original-sample2.png", new [] {Utils.MakeFakeDownloadResult(new MemoryStream(bytes), "original-sample2.png", "image/png", null, "0"),
                Utils.MakeFakeDownloadResult(new MemoryStream(bytes.Reverse().ToArray()),
                                        "original-sample2.png",
                                        "image/png", null, "1")} },
                {"miniature-300x300-sample1.jpeg", new [] {Utils.MakeFakeDownloadResult(new MemoryStream(bytes), "miniature-300x300-sample1.jpeg", "image/jpeg") }},
                {"miniature-200x200-sample1.jpeg", new [] {Utils.MakeFakeDownloadResult(new MemoryStream(bytes), "miniature-200x200-sample1.jpeg", "image/jpeg") }},
                {"miniature-400x400-sample2.jpeg", new [] {Utils.MakeFakeDownloadResult(new MemoryStream(bytes), "miniature-400x400-sample2.jpeg", "image/jpeg") }}
            };
        }
        
        public string BlobName
        {
            get
            {
                return _blobName;
            }
            set
            {
                _blobName = value;
            }
        }

        public async override Task<Azure.Response<BlobContentInfo>> UploadAsync(Stream fileStream, BlobUploadOptions blobOptions, CancellationToken ct)
        {
            if (Utils.Repository.ContainsKey(_blobName))
            {
                var updated = Utils.MakeFakeDownloadResult(fileStream, _blobName, blobOptions.HttpHeaders.ContentType, blobOptions.Metadata);
                var blob = Utils.Repository[_blobName];
                Utils.Repository[_blobName] = blob.Prepend(updated).ToArray();
            }
            else
            {
                Utils.Repository.Add(_blobName,
                    new[] { Utils.MakeFakeDownloadResult(fileStream, _blobName, blobOptions.HttpHeaders.ContentType, blobOptions.Metadata) });
            }

            var mock = new Mock<Azure.Response<BlobContentInfo>>();
            mock.Setup(x => x.GetRawResponse().Status).Returns(201);

            return mock.Object;
        }

        public override Azure.Response<BlobProperties> GetProperties(BlobRequestConditions conditions = null, CancellationToken cancellationToken = default)
        {
            var blob = Utils.Repository[_blobName][0];
            var mock = new Mock<Azure.Response<BlobProperties>>();
            mock.Setup(x => x.Value).Returns(BlobsModelFactory.BlobProperties(default, default, blob.Details.ContentLength, blob.Details.ContentType, default, default, blob.Details.ContentEncoding));

            return mock.Object;
        }

        public new BlobBaseClient WithVersion(string versionId)
        {
            return new BlobBaseClient(_blobName, versionId);
        }

        public async override Task<Stream> OpenReadAsync(BlobOpenReadOptions options, CancellationToken cancellationToken = default)
        {
            var contentArray = Utils.Repository[_blobName][int.Parse(_version)].Content.ToArray();

            return new MemoryStream(contentArray);
        }

        public async override Task<Azure.Response<BlobDownloadResult>> DownloadContentAsync()
        {
            var mock = new Mock<Azure.Response<BlobDownloadResult>>();
            var blobDownloadResult = Utils.Repository[_blobName][int.Parse(_version)];
            
            mock.Setup(x => x.Value).Returns(blobDownloadResult);

            return mock.Object;
        }

        public async override Task<Azure.Response<BlobInfo>> SetMetadataAsync(IDictionary<string, string> metadata, BlobRequestConditions conditions = null, CancellationToken cancellationToken = default)
        {
            var mock = new Mock<Azure.Response<BlobInfo>>();
            mock.Setup(x => x.GetRawResponse().Status).Returns(200);

            var original = Utils.Repository[_blobName][0];
            Utils.Repository[_blobName][0] = Utils.MakeFakeDownloadResult(original.Content.ToStream(), _blobName, original.Details.ContentType, metadata);

            return mock.Object;
        }
    }
}
