using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Application.AbstractServices;

public interface IExchangeRateService
{
    Task<Guid> CreateAsync(CreateExchangeRateDto dto, CancellationToken ct = default);
    Task UpdateAsync(Guid id, CreateExchangeRateDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ExchangeRateDto>> GetAllAsync(CancellationToken ct = default);
    Task<ExchangeRateDto?> GetEffectiveAsync(Currency from, Currency to, DateOnly date, CancellationToken ct = default);
    decimal Convert(Currency from, Currency to, decimal amount, DateOnly date);
}