using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Application.DTOs.PriceCurrency;

public class PriceCurrencyDto
{
    public Guid Id { get; set; }
    public Currency Currency { get; set; }
    public decimal Amount { get; set; }
    public decimal? ExchangeRateToGel { get; set; }
    public DateOnly EffectiveDate { get; set; }
    public decimal? PriceInGel { get; set; }
}