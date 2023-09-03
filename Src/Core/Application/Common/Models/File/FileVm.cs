using Azure.Storage.Blobs.Models;

namespace Application.Common.Models.File;

public class FileVm
{
    /// <summary>
    /// File
    /// </summary>
    public BlobDownloadResult File { get; set; }
}