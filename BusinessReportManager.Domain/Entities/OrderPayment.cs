using BusinessReportsManager.Domain.Common;
using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Domain.Entities;

public class Payment
{
    public int Id { get; set; }
    public Money Amount { get; set; } = new Money(0, Currency.GEL);
    public int BankId { get; set; }
    public Bank? Bank { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
}

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty; // Unique
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedByUserId { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Open;
    public string AccountantComment { get; set; } = string.Empty;
    public OrderSource Source { get; set; } = OrderSource.Other;

    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public int OrderPartyId { get; set; }
    public OrderParty? OrderParty { get; set; }

    public int TourId { get; set; }
    public Tour? Tour { get; set; }

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public decimal GetTotalPaid()
    {
        return Payments.Sum(p => p.Amount.Amount);
    }

    public string PaymentStatus()
    {
        var total = Tour?.Price.Amount ?? 0m;
        var paid = GetTotalPaid();
        if (paid <= 0) return "NotPaid";
        if (paid > 0 && paid < total) return "Partial";
        return "Paid";
    }
}