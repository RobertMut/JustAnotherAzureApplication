using Application.Common.Interfaces.Blob;
using Azure.Storage.Blobs.Models;
using Common.Images;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace Functions.UnitTests.Common
{
    public class CustomTestStartup
    {
        private IDictionary<string, BlobDownloadResult> _blobs;

        public CustomTestStartup()
        {
            _blobs = new Dictionary<string, BlobDownloadResult>();
            Builder = new HostBuilder()
                .ConfigureWebJobs()
                .ConfigureServices(OverrideServices)
                .Build();
        }

        public IHost Builder { get; }

        private void OverrideServices(IServiceCollection services)
        {
            var mock = new Mock<IBlobManagerService>();

            mock.Setup(x => x.AddAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<CancellationToken>()))
                .Callback<Stream, string, string, IDictionary<string, string>, CancellationToken>((fileStream, filename, contentType, metadata, ct) =>
                {
                    _blobs.Add(filename, MakeFakeDownloadResult(fileStream, contentType, metadata));
                }).ReturnsAsync(HttpStatusCode.Created);
            mock.Setup(x => x.DownloadAsync(It.IsAny<string>(), It.IsAny<int?>()))
                .ReturnsAsync((string filename, int? id) =>
                {
                    return _blobs[filename];
                });

            services.AddSingleton<IBlobManagerService>(provider => mock.Object);
            //services.AddSingleton<IBlobMiniatureService>(provider => manager);
            services.AddScoped<ISupportedImageFormats, FunctionImageFormats>();
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
