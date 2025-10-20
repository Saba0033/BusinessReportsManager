using System.ComponentModel.DataAnnotations;
using BusinessReportsManager.Domain.Common;
using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Application;

public record MoneyDto([Range(0, double.MaxValue)] decimal Amount, Currency Currency);

public class CreateSupplierDto
{
    [Required] public string Name { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class SupplierSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int OrdersCount { get; set; }
}

public class TourLocationDto
{
    [Required] public string Country { get; set; } = string.Empty;
    [Required] public string City { get; set; } = string.Empty;
}

public class FlightSegmentDto
{
    public string Airline { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public DateTime Departure { get; set; }
    public DateTime Arrival { get; set; }
}

public class HotelStayDto
{
    public string HotelName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public string? RoomType { get; set; }
}

public class PassengerDto
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Surname { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; } // null allowed for additional
    public string? PassportNumber { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsAdditional { get; set; }
}

public class ExtraServiceDto
{
    public string Name { get; set; } = string.Empty;
    public MoneyDto? Price { get; set; }
}

public class CreateTourDto
{
    public List<FlightSegmentDto> Flights { get; set; } = new();
    public List<HotelStayDto> Hotels { get; set; } = new();
    public List<TourLocationDto> Locations { get; set; } = new();
    [Range(1, int.MaxValue)] public int PassengerCount { get; set; }
    public List<PassengerDto> Passengers { get; set; } = new();
    public List<ExtraServiceDto> ExtraServices { get; set; } = new();
    [Required] public MoneyDto Price { get; set; } = new(0, Currency.GEL);
    [Required] public MoneyDto TicketsSelfCost { get; set; } = new(0, Currency.GEL);
    public decimal? ExchangeRateToGel { get; set; }
}

public class CreateOrderDto
{
    [Required] public string OrdererType { get; set; } = "Person"; // "Person" | "Company"
    // Person fields
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? IdNumber { get; set; }

    // Company fields
    public string? CompanyName { get; set; }
    public string? TaxId { get; set; }
    public string? ContactPerson { get; set; }

    // Shared orderer fields
    public string? Phone { get; set; }
    public string? Email { get; set; }

    [Required] public int SupplierId { get; set; }
    [Required] public OrderSource Source { get; set; } = OrderSource.Other;
    [Required] public CreateTourDto Tour { get; set; } = new();

    public string AccountantComment { get; set; } = string.Empty;
}

public class UpdateOrderDto
{
    public OrderSource? Source { get; set; }
    public string? AccountantComment { get; set; }
    public CreateTourDto? Tour { get; set; }
    public int? SupplierId { get; set; }
}

public class CreatePaymentDto
{
    [Required] public MoneyDto Amount { get; set; } = new(0, Currency.GEL);
    [Required] public int BankId { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class CreateExchangeRateDto
{
    [Required] public Currency Currency { get; set; }
    [Range(0.00001, double.MaxValue)] public decimal RateToGel { get; set; }
    public DateTime? EffectiveDate { get; set; }
}

public class RegisterDto
{
    [Required] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    [Required] public string Role { get; set; } = string.Empty; // Employee | Accountant | Supervisor
}

public class LoginDto
{
    [Required] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}

public class AuthResultDto
{
    public bool Succeeded { get; set; }
    public string? Token { get; set; }
    public string? Error { get; set; }
}