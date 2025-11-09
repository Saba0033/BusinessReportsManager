using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Application.DTOs;

public record ExchangeRateDto(Guid Id, Currency FromCurrency, Currency ToCurrency, decimal Rate, DateOnly EffectiveDate);
public record CreateExchangeRateDto(Currency FromCurrency, Currency ToCurrency, decimal Rate, DateOnly EffectiveDate);