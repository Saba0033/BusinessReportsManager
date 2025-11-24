using BusinessReportsManager.Application.DTOs.PriceCurrency;

namespace BusinessReportsManager.Application.DTOs.HotelBooking;

public class HotelBookingDto
{
    public string HotelName { get; set; } = string.Empty;
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }

    public PriceCurrencyDto Price { get; set; } = new();
}