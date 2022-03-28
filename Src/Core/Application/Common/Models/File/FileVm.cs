using Azure.Storage.Blobs.Models;

namespace Application.Common.Models.File
{
    public class FileVm
    {
        public BlobDownloadResult File { get; set; }
    }
}
