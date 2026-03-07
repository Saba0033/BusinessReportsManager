using BusinessReportsManager.Application.DTOs.PriceCurrency;

namespace BusinessReportsManager.Application.DTOs.AirTicket;

public class AirTicketCreateDto
{
    public string CountryTo { get; set; } = string.Empty;
    public string CityTo { get; set; } = string.Empty;
    public DateOnly FlightDate { get; set; }
    public PriceCurrencyCreateDto Price { get; set; } = new();
}