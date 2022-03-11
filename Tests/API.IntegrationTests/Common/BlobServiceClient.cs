namespace API.IntegrationTests.Common
{
    public class BlobServiceClient : Azure.Storage.Blobs.BlobServiceClient
    {
        public BlobServiceClient()
        {
        }

        public override BlobContainerClient GetBlobContainerClient(string blobContainerName)
        {
            return new BlobContainerClient();
        }
    }
}
