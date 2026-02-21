using BusinessReportsManager.Application.DTOs.PriceCurrency;

namespace BusinessReportsManager.Application.DTOs.HotelBooking;

public class HotelBookingCreateDto
{
    
    public PriceCurrencyCreateDto Price { get; set; } = new();
}