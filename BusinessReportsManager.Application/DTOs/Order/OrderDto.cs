using BusinessReportsManager.Application.DTOs.OrderParty;
using BusinessReportsManager.Application.DTOs.Payment;
using BusinessReportsManager.Application.DTOs.Tour;
using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Application.DTOs.Order;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid? CreatedById { get; set; }
    public string? CreatedByEmail { get; set; } = string.Empty;
    public string? ManagerName { get; set; }

    public int OrderNumber { get; set; }
    public string Source { get; set; } = string.Empty;
    public string? TourType { get; set; }
    public decimal SellPriceInGel { get; set; }
    public decimal TotalExpenseInGel { get; set; }
    public OrderStatus Status { get; set; }
    public string? AccountingComment { get; set; }
    public DateTime? AccountingCommentUpdatedAtUtc { get; set; }
    public Guid? AccountingCommentUpdatedById { get; set; }
    public string? AccountingCommentUpdatedByEmail { get; set; }

    public decimal TicketNet { get; set; }
    public Currency TicketNetCurrency { get; set; }
    public decimal TicketNetRate { get; set; }
    public decimal TicketNetInGel { get; set; }
    public string? TicketSupplier { get; set; }

    public decimal HotelNet { get; set; }
    public Currency HotelNetCurrency { get; set; }
    public decimal HotelNetRate { get; set; }
    public decimal HotelNetInGel { get; set; }
    public string? HotelSupplier { get; set; }

    public decimal TransferNet { get; set; }
    public Currency TransferNetCurrency { get; set; }
    public decimal TransferNetRate { get; set; }
    public decimal TransferNetInGel { get; set; }
    public string? TransferSupplier { get; set; }

    public decimal CruiseNet { get; set; }
    public Currency CruiseNetCurrency { get; set; }
    public decimal CruiseNetRate { get; set; }
    public decimal CruiseNetInGel { get; set; }
    public string? CruiseSupplier { get; set; }

    public decimal InsuranceNet { get; set; }
    public string? InsuranceSupplier { get; set; }

    public decimal OtherServiceNet { get; set; }
    public Currency OtherServiceNetCurrency { get; set; }
    public decimal OtherServiceNetRate { get; set; }
    public decimal OtherServiceNetInGel { get; set; }
    public string? OtherServiceSupplier { get; set; }

    public decimal PaidByClient { get; set; }
    public decimal LeftToPay { get; set; }
    public decimal Profit { get; set; }

    public PartyDto Party { get; set; } = new();
    public TourDto Tour { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
}
