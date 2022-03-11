using Azure.Storage.Blobs.Models;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace API.IntegrationTests.Common
{
    public class BlobContainerClient : Azure.Storage.Blobs.BlobContainerClient
    {
        private BlobClient _blobClient;

        public BlobContainerClient()
        {
            _blobClient = new BlobClient();
        }

        public override BlobClient GetBlobClient(string blobName)
        {
            _blobClient.BlobName = blobName;

            return _blobClient;
        }

        public async override Task<Azure.Response> DeleteBlobAsync(string filename, DeleteSnapshotsOption snapshotsOption = DeleteSnapshotsOption.None, BlobRequestConditions conditions = null, CancellationToken cancellationToken = default)
        {
            var mock = new Mock<Azure.Response>();
            mock.Setup(x => x.Status).Returns(202);

            Utils.Repository.Remove(filename);

            return mock.Object;
        }

        public override Azure.Pageable<BlobItem> GetBlobs(BlobTraits blobTraits = BlobTraits.None, BlobStates states = BlobStates.None, string prefix = null, CancellationToken cancellationToken = default)
        {
            var list = new List<BlobItem>();

            foreach (var item in Utils.Repository)
            {
                for (int i = 0; i < item.Value.Length; i++)
                {
                    list.Add(BlobsModelFactory.BlobItem(item.Key, false,
                    Utils.MakeFakeBlobProperties(item.Value[i].Details.ContentType, item.Value[i].Details.ContentEncoding),
                    null, item.Value[i].Details.VersionId, null, item.Value[i].Details.Metadata, null, null, null));
                }
            }
            
            return new Pageable<BlobItem>(list);
        }
    }
}
