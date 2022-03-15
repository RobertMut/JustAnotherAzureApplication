using Azure.Storage.Blobs.Models;

namespace Application.Images.Queries.GetFile
{
    public class FileVm
    {
        public BlobDownloadResult File { get; set; }
    }
}
