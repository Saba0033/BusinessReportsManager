namespace BusinessReportsManager.Application.Abstractions;

public interface ISupplierService
{
    Task<Supplier> CreateSupplierAsync(CreateSupplierDto dto, CancellationToken ct = default);
    Task<List<SupplierSummaryDto>> GetSuppliersWithCountsAsync(CancellationToken ct = default);
    Task<List<Supplier>> GetAllAsync(CancellationToken ct = default);
}