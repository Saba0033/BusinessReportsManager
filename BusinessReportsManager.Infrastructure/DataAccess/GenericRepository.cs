using System.Linq.Expressions;
using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessReportsManager.Infrastructure.DataAccess;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _db;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _db = context.Set<T>();
    }

    public IQueryable<T> Query(Expression<Func<T, bool>>? filter = null,
        bool asNoTracking = true)
    {
        IQueryable<T> q = _db;

        if (filter != null)
            q = q.Where(filter);

        if (asNoTracking)
            q = q.AsNoTracking();

        return q;
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _db.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(T entity)
    {
        await _db.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _db.AddRangeAsync(entities);
    }

    public Task UpdateAsync(T entity)
    {
        _db.Update(entity);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(T entity)
    {
        _db.Remove(entity);
        return Task.CompletedTask;
    }

    public Task RemoveRangeAsync(IEnumerable<T> entities)
    {
        _db.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}