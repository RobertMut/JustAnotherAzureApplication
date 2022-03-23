using Azure.Storage.Blobs.Specialized;

namespace Application.Common.Interfaces.Image
{
    public interface IImageEditor
    {
        Task<string> Resize(BlobBaseClient blob, Stream stream, string name);
    }
}
