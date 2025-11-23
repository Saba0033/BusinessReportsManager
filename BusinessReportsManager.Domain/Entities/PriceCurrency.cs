using System.ComponentModel.DataAnnotations.Schema;
using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Domain.Entities;

public class PriceCurrency : BaseEntity
{
    public Currency Currency { get; set; }

    public decimal Amount { get; set; }

    public decimal? ExchangeRateToGel { get; set; } = 1;

    public DateOnly EffectiveDate { get; set; }


    [NotMapped] 
    public decimal? PriceInGel => Amount * ExchangeRateToGel;

}