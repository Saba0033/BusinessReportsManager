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

    public PersonPartyDto? PersonParty { get; set; }
    public CompanyPartyDto? CompanyParty { get; set; }

    public TourDto Tour { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
}