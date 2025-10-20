using BusinessReportsManager.Domain.Common;

namespace BusinessReportsManager.Domain.Entities;

public class Passenger
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; } // May be null for additional passengers
    public string? PassportNumber { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsAdditional { get; set; }
}

public class ExtraService
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Money? Price { get; set; } // optional price for service
}

public class FlightSegment
{
    public int Id { get; set; }
    public string Airline { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public DateTime Departure { get; set; }
    public DateTime Arrival { get; set; }
}

public class HotelStay
{
    public int Id { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public string? RoomType { get; set; }
}

public class TourLocation
{
    public int Id { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}

public class Tour
{
    public int Id { get; set; }
    public ICollection<FlightSegment> Flights { get; set; } = new List<FlightSegment>();
    public ICollection<HotelStay> Hotels { get; set; } = new List<HotelStay>();
    public ICollection<TourLocation> Locations { get; set; } = new List<TourLocation>();
    public int PassengerCount { get; set; }
    public ICollection<Passenger> Passengers { get; set; } = new List<Passenger>(); // includes both primary and additional
    public ICollection<ExtraService> ExtraServices { get; set; } = new List<ExtraService>();

    public Money Price { get; set; } = new Money(0, Currency.GEL); // Selling price
    public Money TicketsSelfCost { get; set; } = new Money(0, Currency.GEL); // Tickets' cost (self-worth)
    public decimal? ExchangeRateToGel { get; set; } // entered snapshot used for this tour (if needed)
}