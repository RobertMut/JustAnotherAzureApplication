using Azure.Storage.Blobs.Models;

namespace Application.Images.Queries
{
    public class FileVm
    {
        public BlobDownloadResult File { get; set; }
    }
}
