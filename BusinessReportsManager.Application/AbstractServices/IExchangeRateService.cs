using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Application.AbstractServices;

/// <summary>
/// Resolves the exchange rate of a foreign currency to GEL.
/// Implementations may use an external source (e.g. National Bank of Georgia)
/// and/or honor a manually supplied rate.
/// </summary>
public interface IExchangeRateService
{
    /// <summary>
    /// Returns the rate that converts 1 unit of <paramref name="currency"/> into GEL
    /// for the given (optional) date. GEL always returns 1.
    /// </summary>
    Task<decimal> GetRateToGelAsync(Currency currency, DateOnly? date = null, CancellationToken ct = default);

    /// <summary>
    /// Resolves the rate to use: a positive manual rate wins; otherwise the rate is
    /// fetched from the external source. GEL always resolves to 1.
    /// </summary>
    Task<decimal> ResolveRateToGelAsync(Currency currency, decimal? manualRate, DateOnly? date = null, CancellationToken ct = default);
}
