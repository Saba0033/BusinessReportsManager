using BusinessReportsManager.Domain.Entities;

namespace BusinessReportsManager.Domain.Interfaces;

public interface IGenericRepository
{
    
    IQueryable<T> Query<T>(bool asNoTracking = true) where T : BaseEntity;
    
    Task<T?> GetByIdAsync<T>(Guid id, CancellationToken ct = default) where T : BaseEntity;
    
    Task AddAsync<T>(T entity, CancellationToken ct = default) where T : BaseEntity;
    
    Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken ct = default) where T : BaseEntity;
    
    Task Update<T>(T entity) where T : BaseEntity;
    
    Task Remove<T>(T entity) where T : BaseEntity;
    
    Task RemoveRange<T>(IEnumerable<T> entities) where T : BaseEntity;
    
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    
    Task SetOriginalRowVersion<T>(T entity, byte[] rowVersion) where T : BaseEntity;
}
