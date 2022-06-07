using Azure.Storage.Blobs.Specialized;

namespace Application.Common.Interfaces.Image
{
    /// <summary>
    /// Determines IImageEditor Interface with Resize to provide resizing an image
    /// </summary>
    public interface IImageEditor
    {
        /// <summary>
        /// Resizes Image
        /// </summary>
        /// <param name="blob">BlobClient</param>
        /// <param name="stream">Stream</param>
        /// <param name="filename">Filename</param>
        /// <param name="userId">User guid</param>
        /// <returns>Miniature name</returns>
        Task<string> Resize(BlobBaseClient blob, Stream stream, string filename, string userId);
    }
}
