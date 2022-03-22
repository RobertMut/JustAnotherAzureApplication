namespace Application.Common.Interfaces.Database
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<T> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        Task RemoveAsync(string id, CancellationToken cancellationToken = default);

        Task<string> AddAsync(object[] args, CancellationToken cancellationToken = default);

        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    }
}
