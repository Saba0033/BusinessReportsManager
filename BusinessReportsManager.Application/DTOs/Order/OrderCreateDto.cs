using BusinessReportsManager.Application.DTOs.AirTicket;
using BusinessReportsManager.Application.DTOs.HotelBooking;
using BusinessReportsManager.Application.DTOs.OrderParty;
using BusinessReportsManager.Application.DTOs.Passenger;
using BusinessReportsManager.Application.DTOs.Payment;
using BusinessReportsManager.Application.DTOs.Supplier;
using BusinessReportsManager.Application.DTOs.Tour;

namespace BusinessReportsManager.Application.DTOs.Order;

public class OrderCreateDto
{
    public PartyCreateDto Party { get; set; } = null!;

    public string Destination { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int PassengerCount { get; set; }

    public SupplierCreateDto Supplier { get; set; } = null!;
    public List<AirTicketCreateDto> AirTickets { get; set; } = new();
    public List<HotelBookingCreateDto> HotelBookings { get; set; } = new();

    public string Source { get; set; } = string.Empty;
    public string? TourType { get; set; }
    public string? ManagerName { get; set; }
    public decimal SellPriceInGel { get; set; }
    public CustomerBankRequisitesCreateDto? CustomerBankRequisites { get; set; }
    public decimal TotalExpenseInGel { get; set; }
    public List<PassengerCreateDto> Passengers { get; set; } = new();

    public decimal TicketNet { get; set; }
    public string? TicketSupplier { get; set; }
    public decimal HotelNet { get; set; }
    public string? HotelSupplier { get; set; }
    public decimal TransferNet { get; set; }
    public string? TransferSupplier { get; set; }
    public decimal InsuranceNet { get; set; }
    public string? InsuranceSupplier { get; set; }
    public decimal OtherServiceNet { get; set; }
    public string? OtherServiceSupplier { get; set; }
}