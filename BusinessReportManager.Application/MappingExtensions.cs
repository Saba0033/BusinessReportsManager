using BusinessReportsManager.Domain.Common;
using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Application;

public static class MappingExtensions
{
    public static Order ToEntity(this CreateOrderDto dto, string createdByUserId, string orderNumber)
    {
        OrderParty party = dto.OrdererType.Equals("Company", StringComparison.OrdinalIgnoreCase)
            ? new CompanyOrderParty
                {
                    CompanyName = dto.CompanyName ?? string.Empty,
                    TaxId = dto.TaxId ?? string.Empty,
                    ContactPerson = dto.ContactPerson ?? string.Empty,
                    Phone = dto.Phone ?? string.Empty,
                    Email = dto.Email ?? string.Empty,
                    DisplayName = dto.CompanyName ?? string.Empty
                }
            : new PersonOrderParty
                {
                    Name = dto.Name ?? string.Empty,
                    Surname = dto.Surname ?? string.Empty,
                    BirthDate = dto.BirthDate,
                    IdNumber = dto.IdNumber ?? string.Empty,
                    Phone = dto.Phone ?? string.Empty,
                    Email = dto.Email ?? string.Empty,
                    DisplayName = $"{dto.Name} {dto.Surname}".Trim()
                };

        var tour = dto.Tour.ToEntity();

        return new Order
        {
            OrderNumber = orderNumber,
            CreatedByUserId = createdByUserId,
            Status = OrderStatus.Open,
            AccountantComment = dto.AccountantComment ?? string.Empty,
            Source = dto.Source,
            SupplierId = dto.SupplierId,
            OrderParty = party,
            Tour = tour
        };
    }

    public static Tour ToEntity(this CreateTourDto dto)
    {
        var tour = new Tour
        {
            PassengerCount = dto.PassengerCount,
            Price = new Money(dto.Price.Amount, dto.Price.Currency),
            TicketsSelfCost = new Money(dto.TicketsSelfCost.Amount, dto.TicketsSelfCost.Currency),
            ExchangeRateToGel = dto.ExchangeRateToGel
        };

        tour.Locations = dto.Locations.Select(l => new TourLocation
        {
            Country = l.Country,
            City = l.City
        }).ToList();

        tour.Flights = dto.Flights.Select(f => new FlightSegment
        {
            Airline = f.Airline,
            FlightNumber = f.FlightNumber,
            FromCity = f.FromCity,
            ToCity = f.ToCity,
            Departure = f.Departure,
            Arrival = f.Arrival
        }).ToList();

        tour.Hotels = dto.Hotels.Select(h => new HotelStay
        {
            HotelName = h.HotelName,
            City = h.City,
            Country = h.Country,
            CheckIn = h.CheckIn,
            CheckOut = h.CheckOut,
            RoomType = h.RoomType
        }).ToList();

        tour.Passengers = dto.Passengers.Select(p => new Passenger
        {
            Name = p.Name,
            Surname = p.Surname,
            BirthDate = p.BirthDate,
            PassportNumber = p.PassportNumber,
            Phone = p.Phone,
            Email = p.Email,
            IsAdditional = p.IsAdditional
        }).ToList();

        tour.ExtraServices = dto.ExtraServices.Select(es => new ExtraService
        {
            Name = es.Name,
            Price = es.Price is null ? null : new Money(es.Price.Amount, es.Price.Currency)
        }).ToList();

        return tour;
    }

    public static void Apply(this Tour entity, CreateTourDto dto)
    {
        entity.PassengerCount = dto.PassengerCount;
        entity.Price = new Money(dto.Price.Amount, dto.Price.Currency);
        entity.TicketsSelfCost = new Money(dto.TicketsSelfCost.Amount, dto.TicketsSelfCost.Currency);
        entity.ExchangeRateToGel = dto.ExchangeRateToGel;

        entity.Locations.Clear();
        entity.Locations = dto.Locations.Select(l => new TourLocation { Country = l.Country, City = l.City }).ToList();

        entity.Flights.Clear();
        entity.Flights = dto.Flights.Select(f => new FlightSegment
        {
            Airline = f.Airline,
            FlightNumber = f.FlightNumber,
            FromCity = f.FromCity,
            ToCity = f.ToCity,
            Departure = f.Departure,
            Arrival = f.Arrival
        }).ToList();

        entity.Hotels.Clear();
        entity.Hotels = dto.Hotels.Select(h => new HotelStay
        {
            HotelName = h.HotelName,
            City = h.City,
            Country = h.Country,
            CheckIn = h.CheckIn,
            CheckOut = h.CheckOut,
            RoomType = h.RoomType
        }).ToList();

        entity.Passengers.Clear();
        entity.Passengers = dto.Passengers.Select(p => new Passenger
        {
            Name = p.Name,
            Surname = p.Surname,
            BirthDate = p.BirthDate,
            PassportNumber = p.PassportNumber,
            Phone = p.Phone,
            Email = p.Email,
            IsAdditional = p.IsAdditional
        }).ToList();

        entity.ExtraServices.Clear();
        entity.ExtraServices = dto.ExtraServices.Select(es => new ExtraService
        {
            Name = es.Name,
            Price = es.Price is null ? null : new Money(es.Price.Amount, es.Price.Currency)
        }).ToList();
    }
}