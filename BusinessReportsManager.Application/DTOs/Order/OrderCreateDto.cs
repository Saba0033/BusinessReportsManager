using BusinessReportsManager.Application.DTOs.AirTicket;
using BusinessReportsManager.Application.DTOs.HotelBooking;
using BusinessReportsManager.Application.DTOs.OrderParty;
using BusinessReportsManager.Application.DTOs.Passenger;
using BusinessReportsManager.Application.DTOs.Payment;
using BusinessReportsManager.Application.DTOs.Supplier;
using BusinessReportsManager.Application.DTOs.Tour;
using BusinessReportsManager.Domain.Enums;

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
    public List<PassengerCreateDto> Passengers { get; set; } = new();

    // ---- Service NETs ----------------------------------------------------
    // For each currency-aware service: amount is in the chosen currency.
    // Currency defaults to GEL. ExchangeRateToGel is optional:
    //   - omit (null) to auto-resolve from the National Bank of Georgia
    //   - provide a positive value to override with a manual rate
    // Insurance is GEL-only and has no currency/rate.
    public decimal TicketNet { get; set; }
    public Currency TicketNetCurrency { get; set; } = Currency.GEL;
    public decimal? TicketNetRate { get; set; }
    public string? TicketSupplier { get; set; }

    public decimal HotelNet { get; set; }
    public Currency HotelNetCurrency { get; set; } = Currency.GEL;
    public decimal? HotelNetRate { get; set; }
    public string? HotelSupplier { get; set; }

    public decimal TransferNet { get; set; }
    public Currency TransferNetCurrency { get; set; } = Currency.GEL;
    public decimal? TransferNetRate { get; set; }
    public string? TransferSupplier { get; set; }

    public decimal CruiseNet { get; set; }
    public Currency CruiseNetCurrency { get; set; } = Currency.GEL;
    public decimal? CruiseNetRate { get; set; }
    public string? CruiseSupplier { get; set; }

    public decimal InsuranceNet { get; set; }
    public string? InsuranceSupplier { get; set; }

    public decimal OtherServiceNet { get; set; }
    public Currency OtherServiceNetCurrency { get; set; } = Currency.GEL;
    public decimal? OtherServiceNetRate { get; set; }
    public string? OtherServiceSupplier { get; set; }
}
