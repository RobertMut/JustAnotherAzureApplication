using Azure;
using Azure.Storage;
using Azure.Storage.Blobs.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace API.IntegrationTests.Common
{
    public class BlobBaseClient : Azure.Storage.Blobs.Specialized.BlobBaseClient
    {
        private readonly string _blobName;
        private readonly string _version;

        public BlobBaseClient(string blobName, string version)
        {
            _blobName = blobName;
            _version = version;
        }

        public override Task<Response> DownloadToAsync(Stream destination)
        {
            return base.DownloadToAsync(destination);
        }

        public override Task<Response> DownloadToAsync(Stream destination, CancellationToken cancellationToken)
        {
            return base.DownloadToAsync(destination, cancellationToken);
        }

        public override Task<Response> DownloadToAsync(Stream destination, BlobRequestConditions conditions = null, StorageTransferOptions transferOptions = default, CancellationToken cancellationToken = default)
        {
            return base.DownloadToAsync(destination, conditions, transferOptions, cancellationToken);
        }

        public async override Task<Stream> OpenReadAsync(BlobOpenReadOptions options, CancellationToken cancellationToken = default)
        {
            var contentArray = Utils.Repository[_blobName][int.Parse(_version)].Content.ToArray();

            return new MemoryStream(contentArray);
        }
    }
}
