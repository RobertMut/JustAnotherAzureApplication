using Azure.Storage.Blobs.Models;

namespace Application.Common.Models.File
{
    /// <summary>
    /// Class FileVm
    /// </summary>
    public class FileVm
    {
        /// <summary>
        /// File
        /// </summary>
        public BlobDownloadResult File { get; set; }
    }
}
