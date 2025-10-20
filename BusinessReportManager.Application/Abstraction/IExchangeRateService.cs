using BusinessReportsManager.Domain.Entities;

namespace BusinessReportsManager.Application.Abstractions;

public interface IExchangeRateService
{
    Task<ExchangeRate> AddAsync(CreateExchangeRateDto dto, CancellationToken ct = default);
    Task<List<ExchangeRate>> GetAllAsync(CancellationToken ct = default);
}