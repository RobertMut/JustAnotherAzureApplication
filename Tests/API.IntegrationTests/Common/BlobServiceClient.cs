namespace API.IntegrationTests.Common
{
    public class BlobServiceClient : Azure.Storage.Blobs.BlobServiceClient
    {


        public BlobServiceClient(string connectionString) : base(connectionString)
        {
        }

        public BlobServiceClient(string connectionString, Azure.Storage.Blobs.BlobClientOptions options) : base(connectionString, options)
        {
        }

        public BlobServiceClient()
        {
        }

        public override BlobContainerClient GetBlobContainerClient(string blobContainerName)
        {
            return new BlobContainerClient();
        }
    }
}
