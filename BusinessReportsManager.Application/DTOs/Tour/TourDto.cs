using BusinessReportsManager.Application.DTOs.AirTicket;
using BusinessReportsManager.Application.DTOs.ExtraService;
using BusinessReportsManager.Application.DTOs.HotelBooking;
using BusinessReportsManager.Application.DTOs.Passenger;
using BusinessReportsManager.Application.DTOs.Supplier;

namespace BusinessReportsManager.Application.DTOs.Tour;

public class TourDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int PassengerCount { get; set; }

    public SupplierDto Supplier { get; set; } = new();

    public List<PassengerDto> Passengers { get; set; } = new();
    public List<AirTicketDto> AirTickets { get; set; } = new();
    public List<HotelBookingDto> HotelBookings { get; set; } = new();
    public List<ExtraServiceDto> ExtraServices { get; set; } = new();
}