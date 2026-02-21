using BusinessReportsManager.Application.DTOs.PriceCurrency;

namespace BusinessReportsManager.Application.DTOs.AirTicket;

public class AirTicketDto
{
    public string CountryFrom { get; set; } = string.Empty;
    public string CountryTo { get; set; } = string.Empty;
    public string CityFrom { get; set; } = string.Empty;
    public string CityTo { get; set; } = string.Empty;
    public DateOnly FlightDate { get; set; }

    public PriceCurrencyDto Price { get; set; } = new();
}