using System.Net;

namespace Application.Common.Interfaces.Blob
{
    public interface IBlobCreatorService
    {
        Task<HttpStatusCode> AddAsync(Stream fileStream, string filename, string contentType, CancellationToken ct);
    }
}
