using BusinessReportsManager.Application.DTOs;

namespace BusinessReportsManager.Application.AbstractServices;

public interface IBankService
{
    Task<Guid> CreateAsync(CreateBankDto dto, CancellationToken ct = default);
    Task UpdateAsync(Guid id, CreateBankDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<BankDto>> GetAllAsync(CancellationToken ct = default);
}