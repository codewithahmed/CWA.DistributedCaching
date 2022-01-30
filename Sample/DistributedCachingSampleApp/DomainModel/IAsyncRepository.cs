namespace DistributedCachingSampleApp.DomainModel;

public interface IAsyncRepository<T>
{
  Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);
  Task<List<T>> ListAllAsync(CancellationToken cancellationToken = default);
  Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
  Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
  Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}

