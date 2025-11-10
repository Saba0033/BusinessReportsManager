using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.Enums;
using BusinessReportsManager.Domain.Interfaces;
using BusinessReportsManager.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BusinessReportsManager.Infrastructure.Services;

public class ExchangeRateService : IExchangeRateService
{
    private readonly IGenericRepository _repo;

    public ExchangeRateService(IGenericRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> CreateAsync(CreateExchangeRateDto dto, CancellationToken ct = default)
    {
        var entity = new ExchangeRate
        {
            FromCurrency = dto.FromCurrency,
            ToCurrency = dto.ToCurrency,
            Rate = dto.Rate,
            EffectiveDate = dto.EffectiveDate
        };
        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(Guid id, CreateExchangeRateDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync<ExchangeRate>(id, ct) ?? throw new KeyNotFoundException();
        entity.FromCurrency = dto.FromCurrency;
        entity.ToCurrency = dto.ToCurrency;
        entity.Rate = dto.Rate;
        entity.EffectiveDate = dto.EffectiveDate;
        await _repo.Update(entity);
        await _repo.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync<ExchangeRate>(id, ct);
        if (entity != null)
        {
            await _repo.Remove(entity);
            await _repo.SaveChangesAsync(ct);
        }
    }

    public async Task<IReadOnlyList<ExchangeRateDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repo.Query<ExchangeRate>()
            .OrderByDescending(x => x.EffectiveDate)
            .Select(x => new ExchangeRateDto(x.Id, x.FromCurrency, x.ToCurrency, x.Rate, x.EffectiveDate))
            .ToListAsync(ct);
    }

    public async Task<ExchangeRateDto?> GetEffectiveAsync(Currency from, Currency to, DateOnly date, CancellationToken ct = default)
    {
        var rate = await _repo.Query<ExchangeRate>()
            .Where(x => x.FromCurrency == from && x.ToCurrency == to && x.EffectiveDate <= date)
            .OrderByDescending(x => x.EffectiveDate)
            .FirstOrDefaultAsync(ct);

        return rate is null ? null : new ExchangeRateDto(rate.Id, rate.FromCurrency, rate.ToCurrency, rate.Rate, rate.EffectiveDate);
    }

    public decimal Convert(Currency from, Currency to, decimal amount, DateOnly date)
    {
        if (from == to) return amount;

        var rate = _repo.Query<ExchangeRate>()
            .Where(x => x.FromCurrency == from && x.ToCurrency == to && x.EffectiveDate <= date)
            .OrderByDescending(x => x.EffectiveDate)
            .FirstOrDefault();

        if (rate is null)
        {
            var inverse = _repo.Query<ExchangeRate>()
                .Where(x => x.FromCurrency == to && x.ToCurrency == from && x.EffectiveDate <= date)
                .OrderByDescending(x => x.EffectiveDate)
                .FirstOrDefault();

            if (inverse is null)
                throw new InvalidOperationException($"No exchange rate from {from} to {to} on or before {date}.");

            return amount / inverse.Rate;
        }

        return amount * rate.Rate;
    }
}
