using BusinessReportsManager.Application.DTOs.AirTicket;
using BusinessReportsManager.Application.DTOs.ExtraService;
using BusinessReportsManager.Application.DTOs.HotelBooking;
using BusinessReportsManager.Application.DTOs.Supplier;

namespace BusinessReportsManager.Application.DTOs.Tour;

public class TourCreateDto
{
    public string Destination{ get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int PassengerCount { get; set; }

    public SupplierCreateDto Supplier { get; set; } = null!;

    public List<AirTicketCreateDto> AirTickets { get; set; } = new();
    public List<HotelBookingCreateDto> HotelBookings { get; set; } = new();
}