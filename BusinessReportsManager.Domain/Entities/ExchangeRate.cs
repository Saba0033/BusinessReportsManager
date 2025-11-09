using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Domain.Entities;

public class ExchangeRate : BaseEntity
{
    public Currency FromCurrency { get; set; }
    public Currency ToCurrency { get; set; }
    public decimal Rate { get; set; }
    public DateOnly EffectiveDate { get; set; }
}