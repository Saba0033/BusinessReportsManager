using BusinessReportsManager.Application.DTOs.OrderParty;
using BusinessReportsManager.Application.DTOs.Passenger;
using BusinessReportsManager.Application.DTOs.Payment;
using BusinessReportsManager.Application.DTOs.Tour;

namespace BusinessReportsManager.Application.DTOs.Order;

public class OrderCreateDto
{
    public PartyCreateDto Party { get; set; } = null!;
    public TourCreateDto Tour { get; set; } = null!;

    public string Source { get; set; } = string.Empty;
    public decimal SellPriceInGel { get; set; }
    public CustomerBankRequisitesCreateDto? CustomerBankRequisites { get; set; }

    public List<PassengerCreateDto> Passengers { get; set; } = new();
}