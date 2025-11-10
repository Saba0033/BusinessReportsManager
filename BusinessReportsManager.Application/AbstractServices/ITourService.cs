using BusinessReportsManager.Application.DTOs;

namespace BusinessReportsManager.Application.AbstractServices;

public interface ITourService
{
    Task<Guid> CreateAsync(CreateTourDto dto, CancellationToken ct = default);
    Task<TourDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TourDto>> GetFilteredAsync(DateOnly? start, DateOnly? end, Guid? supplierId, string? destination, CancellationToken ct = default);
    Task UpdateAsync(Guid id, CreateTourDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}