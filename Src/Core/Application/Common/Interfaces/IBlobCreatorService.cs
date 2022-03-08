namespace Application.Common.Interfaces
{
    public interface IBlobCreatorService
    {
        Task<int> AddAsync(Stream fileStream, string filename, string contentType, CancellationToken ct);
    }
}
