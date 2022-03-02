namespace Application.Common.Interfaces
{
    public interface IBlobManagerService
    {
        Task<int> AddAsync(Stream fileStream, string filename, string contentType, CancellationToken ct);
    }
}
