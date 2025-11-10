using BusinessReportsManager.Application.DTOs;

namespace BusinessReportsManager.Application.AbstractServices;

public interface ISupplierService
{
    Task<Guid> CreateAsync(CreateSupplierDto dto, CancellationToken ct = default);
    Task UpdateAsync(Guid id, CreateSupplierDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<SupplierDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<SupplierDto>> GetAllAsync(CancellationToken ct = default);
}