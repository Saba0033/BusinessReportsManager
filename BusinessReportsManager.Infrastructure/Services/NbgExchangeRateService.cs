using System.Collections.Concurrent;
using System.Text.Json;
using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Infrastructure.Services;

/// <summary>
/// Exchange rate provider backed by the National Bank of Georgia (NBG) public API.
/// Rates are cached in-memory per (currency, date). A manually supplied rate always
/// takes precedence over the fetched one.
/// </summary>
public class NbgExchangeRateService : IExchangeRateService
{
    private readonly HttpClient _http;
    private static readonly ConcurrentDictionary<string, decimal> _cache = new();

    public NbgExchangeRateService(HttpClient http)
    {
        _http = http;
    }

    public async Task<decimal> ResolveRateToGelAsync(Currency currency, decimal? manualRate, DateOnly? date = null, CancellationToken ct = default)
    {
        if (currency == Currency.GEL)
            return 1m;

        if (manualRate is > 0)
            return manualRate.Value;

        return await GetRateToGelAsync(currency, date, ct);
    }

    public async Task<decimal> GetRateToGelAsync(Currency currency, DateOnly? date = null, CancellationToken ct = default)
    {
        if (currency == Currency.GEL)
            return 1m;

        var code = currency.ToString().ToUpperInvariant();
        var cacheKey = $"{code}:{date?.ToString("yyyy-MM-dd") ?? "latest"}";

        if (_cache.TryGetValue(cacheKey, out var cached))
            return cached;

        var rate = await FetchFromNbgAsync(code, date, ct);
        if (rate is > 0)
        {
            _cache[cacheKey] = rate.Value;
            return rate.Value;
        }

        throw new InvalidOperationException(
            $"Could not resolve exchange rate for {code} to GEL. Provide the rate manually (exchangeRateToGel).");
    }

    private async Task<decimal?> FetchFromNbgAsync(string code, DateOnly? date, CancellationToken ct)
    {
        try
        {
            var url = $"https://nbg.gov.ge/gw/api/ct/monetarypolicy/currencies/en/json/?currencies={code}";
            if (date.HasValue)
                url += $"&date={date.Value:yyyy-MM-dd}";

            using var resp = await _http.GetAsync(url, ct);
            if (!resp.IsSuccessStatusCode)
                return null;

            await using var stream = await resp.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            // Response shape: [ { "date": "...", "currencies": [ { "code": "USD", "rate": 2.71, "quantity": 1 } ] } ]
            foreach (var bucket in doc.RootElement.EnumerateArray())
            {
                if (!bucket.TryGetProperty("currencies", out var currencies))
                    continue;

                foreach (var c in currencies.EnumerateArray())
                {
                    if (!c.TryGetProperty("code", out var codeEl) ||
                        !string.Equals(codeEl.GetString(), code, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var rate = c.GetProperty("rate").GetDecimal();
                    var quantity = c.TryGetProperty("quantity", out var q) ? q.GetDecimal() : 1m;
                    if (quantity <= 0) quantity = 1m;

                    // NBG quotes "rate" GEL per "quantity" units; normalize to per 1 unit.
                    return rate / quantity;
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
