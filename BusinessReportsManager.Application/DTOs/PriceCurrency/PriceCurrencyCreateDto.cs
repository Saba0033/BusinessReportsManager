using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Application.DTOs.PriceCurrency;

public class PriceCurrencyCreateDto
{
    public Currency Currency { get; set; }
    public decimal Amount { get; set; }
    public decimal? ExchangeRateToGel { get; set; }
}
