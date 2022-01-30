using DistributedCachingSampleApp.DomainModel;
using Microsoft.EntityFrameworkCore;
namespace DistributedCachingSampleApp.Data;

public class EfRepository<T> : IAsyncRepository<T> where T : BaseEntity
{
  private readonly AppDbContext _dbContext;

  public EfRepository(AppDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default)
  {
    return await _dbContext.Set<T>()
      .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
  }

  public async Task<List<T>> ListAllAsync(CancellationToken cancellationToken = default)
  {
    return await _dbContext.Set<T>().ToListAsync(cancellationToken);
  }

  public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
  {
    await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);

    return entity;
  }

  public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
  {
    _dbContext.Entry(entity).State = EntityState.Modified;
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
  {
    _dbContext.Set<T>().Remove(entity);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }
}

