using BusinessReportsManager.Application.DTOs;

namespace BusinessReportsManager.Application.AbstractServices;

public interface IOrderPartyService
{
    Task<Guid> CreatePersonAsync(CreatePersonPartyDto dto, CancellationToken ct = default);
    Task<Guid> CreateCompanyAsync(CreateCompanyPartyDto dto, CancellationToken ct = default);
    Task<OrderPartyDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<OrderPartyDto>> GetAllAsync(CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}