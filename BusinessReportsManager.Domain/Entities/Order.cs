using System.ComponentModel.DataAnnotations.Schema;
using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Domain.Entities;

public class Order : BaseEntity
{
    public int OrderNumber { get; set; }
    public Guid OrderPartyId { get; set; }
    public Guid? CreatedById { get; set; }
    public string? CreatedByEmail { get; set; } = string.Empty;
    public string? ManagerName { get; set; }
    public OrderParty? OrderParty { get; set; }
    public Guid TourId { get; set; }
    public Tour? Tour { get; set; }
    public string Source { get; set; } = string.Empty;
    public string? TourType { get; set; }
    public decimal SellPriceInGel { get; set; } = 0;

    // Total of every service NET converted to GEL. Recomputed on create/edit.
    public decimal TotalExpenseInGel { get; set; } = 0;

    // ---- Service NETs ----------------------------------------------------
    // Each currency-aware NET stores the amount in its own currency together
    // with the exchange rate used to convert it to GEL at booking time.
    // Insurance is GEL-only by business rule.
    public decimal TicketNet { get; set; }
    public Currency TicketNetCurrency { get; set; } = Currency.GEL;
    public decimal TicketNetRate { get; set; } = 1m;
    public string? TicketSupplier { get; set; }

    public decimal HotelNet { get; set; }
    public Currency HotelNetCurrency { get; set; } = Currency.GEL;
    public decimal HotelNetRate { get; set; } = 1m;
    public string? HotelSupplier { get; set; }

    public decimal TransferNet { get; set; }
    public Currency TransferNetCurrency { get; set; } = Currency.GEL;
    public decimal TransferNetRate { get; set; } = 1m;
    public string? TransferSupplier { get; set; }

    public decimal CruiseNet { get; set; }
    public Currency CruiseNetCurrency { get; set; } = Currency.GEL;
    public decimal CruiseNetRate { get; set; } = 1m;
    public string? CruiseSupplier { get; set; }

    public decimal InsuranceNet { get; set; }
    public string? InsuranceSupplier { get; set; }

    public decimal OtherServiceNet { get; set; }
    public Currency OtherServiceNetCurrency { get; set; } = Currency.GEL;
    public decimal OtherServiceNetRate { get; set; } = 1m;
    public string? OtherServiceSupplier { get; set; }

    [NotMapped] public decimal TicketNetInGel => TicketNet * TicketNetRate;
    [NotMapped] public decimal HotelNetInGel => HotelNet * HotelNetRate;
    [NotMapped] public decimal TransferNetInGel => TransferNet * TransferNetRate;
    [NotMapped] public decimal CruiseNetInGel => CruiseNet * CruiseNetRate;
    [NotMapped] public decimal InsuranceNetInGel => InsuranceNet;
    [NotMapped] public decimal OtherServiceNetInGel => OtherServiceNet * OtherServiceNetRate;

    [NotMapped]
    public decimal ComputedTotalExpenseInGel =>
        TicketNetInGel + HotelNetInGel + TransferNetInGel +
        CruiseNetInGel + InsuranceNetInGel + OtherServiceNetInGel;

    public string? AccountingComment { get; set; }
    public DateTime? AccountingCommentUpdatedAtUtc { get; set; }
    public Guid? AccountingCommentUpdatedById { get; set; }
    public string? AccountingCommentUpdatedByEmail { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Open;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}