using System.Linq.Expressions;
using BusinessReportsManager.Domain.Entities;

namespace BusinessReportsManager.Domain.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    IQueryable<T> Query(Expression<Func<T, bool>>? filter = null,
        bool asNoTracking = true);

    Task<T?> GetByIdAsync(Guid id);

    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);

    Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);
    Task RemoveRangeAsync(IEnumerable<T> entities);

    Task<int> SaveChangesAsync();
}