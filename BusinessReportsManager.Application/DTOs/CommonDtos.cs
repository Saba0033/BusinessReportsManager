namespace BusinessReportsManager.Application.DTOs;

using BusinessReportsManager.Domain.Enums;
using System;
using System.Collections.Generic;

//
// =============================================================
// ORDER (ROOT)
// =============================================================
//

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public decimal SellPriceInGel { get; set; }
    public OrderStatus Status { get; set; }

    public PersonPartyDto? PersonParty { get; set; }
    public CompanyPartyDto? CompanyParty { get; set; }

    public TourDto Tour { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
}

public class OrderCreateDto
{
    public PartyCreateDto Party { get; set; } = null!;
    public TourCreateDto Tour { get; set; } = null!;

    public string Source { get; set; } = string.Empty;
    public decimal SellPriceInGel { get; set; }

    public List<PassengerCreateDto> Passengers { get; set; } = new();
    public List<PaymentCreateDto> Payments { get; set; } = new();
}

public class OrderEditDto
{
    public decimal? SellPriceInGel { get; set; }
    public string? Source { get; set; }
}

//
// =============================================================
// ORDER PARTIES (PERSON + COMPANY)
// =============================================================
//

public class PartyCreateDto
{
    public string Type { get; set; } = "Person";   // "Person" or "Company"

    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    // Person fields
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly? BirthDate { get; set; }

    // Company fields
    public string? CompanyName { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? ContactPerson { get; set; }
}

public class PersonPartyDto
{
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
}

public class CompanyPartyDto
{
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    public string CompanyName { get; set; } = string.Empty;
    public string? RegistrationNumber { get; set; }
    public string? ContactPerson { get; set; }
}

//
// =============================================================
// TOUR
// =============================================================
//

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

public class TourCreateDto
{
    public string Name { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int PassengerCount { get; set; }

    public SupplierCreateDto Supplier { get; set; } = null!;

    public List<AirTicketCreateDto> AirTickets { get; set; } = new();
    public List<HotelBookingCreateDto> HotelBookings { get; set; } = new();
    public List<ExtraServiceCreateDto> ExtraServices { get; set; } = new();
}

//
// =============================================================
// PASSENGER
// =============================================================
//

public class PassengerDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
    public string? DocumentNumber { get; set; }
}

public class PassengerCreateDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
    public string? DocumentNumber { get; set; }
}

//
// =============================================================
// SUPPLIER
// =============================================================
//

public class SupplierDto
{
    public string Name { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
    public string? Phone { get; set; }
}

public class SupplierCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
    public string? Phone { get; set; }
}

//
// =============================================================
// AIR TICKETS
// =============================================================
//

public class AirTicketDto
{
    public string CountryFrom { get; set; } = string.Empty;
    public string CountryTo { get; set; } = string.Empty;
    public string CityFrom { get; set; } = string.Empty;
    public string CityTo { get; set; } = string.Empty;
    public DateOnly FlightDate { get; set; }
    public string FlightCompanyName { get; set; } = string.Empty;
    public int Quantity { get; set; }

    public PriceCurrencyDto Price { get; set; } = new();
}

public class AirTicketCreateDto
{
    public string CountryFrom { get; set; } = string.Empty;
    public string CountryTo { get; set; } = string.Empty;
    public string CityFrom { get; set; } = string.Empty;
    public string CityTo { get; set; } = string.Empty;
    public DateOnly FlightDate { get; set; }
    public string FlightCompanyName { get; set; } = string.Empty;
    public int Quantity { get; set; }

    public PriceCurrencyCreateDto Price { get; set; } = new();
}

//
// =============================================================
// HOTEL BOOKINGS
// =============================================================
//

public class HotelBookingDto
{
    public string HotelName { get; set; } = string.Empty;
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }

    public PriceCurrencyDto Price { get; set; } = new();
}

public class HotelBookingCreateDto
{
    public string HotelName { get; set; } = string.Empty;
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }

    public PriceCurrencyCreateDto Price { get; set; } = new();
}

//
// =============================================================
// EXTRA SERVICES
// =============================================================
//

public class ExtraServiceDto
{
    public string Description { get; set; } = string.Empty;
    public PriceCurrencyDto Price { get; set; } = new();
}

public class ExtraServiceCreateDto
{
    public string Description { get; set; } = string.Empty;
    public PriceCurrencyCreateDto Price { get; set; } = new();
}

//
// =============================================================
// PRICE CURRENCY
// =============================================================
//

public class PriceCurrencyDto
{
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public decimal? ExchangeRateToGel { get; set; }
    public DateOnly EffectiveDate { get; set; }
}

public class PriceCurrencyCreateDto
{
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public decimal? ExchangeRateToGel { get; set; }
}

//
// =============================================================
// PAYMENT
// =============================================================
//

public class PaymentCreateDto
{
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public string? BankName { get; set; }
    public string? Reference { get; set; }
}

public class PaymentDto
{
    public Guid Id { get; set; }
    public PriceCurrencyDto Price { get; set; } = new();
    public DateOnly PaidDate { get; set; }
    public string? BankName { get; set; }
    public string? Reference { get; set; }
}
