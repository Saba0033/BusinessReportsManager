using BusinessReportsManager.Application.DTOs.PriceCurrency;

namespace BusinessReportsManager.Application.DTOs.HotelBooking;

public class HotelBookingDto
{
    public PriceCurrencyDto Price { get; set; } = new();
}