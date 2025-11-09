using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessReportsManager.Infrastructure.DataAccess;


public class GenericRepository : IGenericRepository
{
    private readonly AppDbContext _context;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<T> Query<T>(bool asNoTracking = true) where T : BaseEntity
    {
        var query = _context.Set<T>().AsQueryable();
        return asNoTracking ? query.AsNoTracking() : query;
    }

    public async Task<T?> GetByIdAsync<T>(Guid id, CancellationToken ct = default) where T : BaseEntity
    {
        return await _context.Set<T>().FindAsync(new object[] { id }, ct);
    }

    public async Task AddAsync<T>(T entity, CancellationToken ct = default) where T : BaseEntity
    {
        await _context.Set<T>().AddAsync(entity, ct);
    }

    public async Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken ct = default) where T : BaseEntity
    {
        await _context.Set<T>().AddRangeAsync(entities, ct);
    }

    public Task Update<T>(T entity) where T : BaseEntity
    {
        _context.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public Task Remove<T>(T entity) where T : BaseEntity
    {
        _context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public Task RemoveRange<T>(IEnumerable<T> entities) where T : BaseEntity
    {
        _context.Set<T>().RemoveRange(entities);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }

    public Task SetOriginalRowVersion<T>(T entity, byte[] rowVersion) where T : BaseEntity
    {
        _context.Entry(entity).OriginalValues["RowVersion"] = rowVersion;
        return Task.CompletedTask;
    }
}