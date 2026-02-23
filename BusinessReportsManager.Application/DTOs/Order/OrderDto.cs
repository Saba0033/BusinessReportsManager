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

    public string OrderNumber { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public decimal SellPriceInGel { get; set; }
    public OrderStatus Status { get; set; }
    public string? AccountingComment { get; set; }
    public DateTime? AccountingCommentUpdatedAtUtc { get; set; }
    public Guid? AccountingCommentUpdatedById { get; set; }
    public string? AccountingCommentUpdatedByEmail { get; set; }

    public PartyDto Party { get; set; } = new();
    public TourDto Tour { get; set; } = new();
    public CustomerBankRequisitesDto? CustomerBankRequisites { get; set; }
    public List<PaymentDto> Payments { get; set; } = new();
}