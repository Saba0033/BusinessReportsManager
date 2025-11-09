using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessReportsManager.Infrastructure.Services;

public class OrderNumberGenerator : IOrderNumberGenerator
{
    private readonly IGenericRepository _repo;
    public OrderNumberGenerator(IGenericRepository repo) => _repo = repo;

    public async Task<string> NextOrderNumberAsync(CancellationToken ct = default)
    {
        var year = DateTime.UtcNow.Year;
        var count = await _repo.Query<Order>()
            .CountAsync(o => o.CreatedAtUtc.Year == year, ct);
        var nextSeq = count + 1;
        return $"ORD-{year}-{nextSeq:0000}";
    }
}
