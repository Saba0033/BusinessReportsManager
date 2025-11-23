using System.ComponentModel.DataAnnotations;
using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Domain.Entities;

public class Tour : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int PassengerCount { get; set; }
    
    public Guid TourSupplierId { get; set; }
    
    public ICollection<AirTicket> AirTickets { get; set; } = new List<AirTicket>();
    
    public ICollection<HotelBooking> HotelBookings { get; set; } = new List<HotelBooking>();
    
    public ICollection<ExtraService> ExtraServices { get; set; } = new List<ExtraService>();

    public Supplier TourSupplier { get; set; } = new Supplier();
}


public class Destination : BaseEntity
{
    public Guid TourId { get; set; }
    
    public Tour? Tour { get; set; }
    
    public string City { get; set; } = string.Empty;
    
    public string Country { get; set; } = string.Empty;
}

public class AirTicket : BaseEntity
{
    public Guid TourId { get; set; }
    public Tour? Tour { get; set; }
    
    public string CountryFrom { get; set; } = string.Empty;
    
    public string CountryTo { get; set; } = string.Empty;
    
    public string CityFrom { get; set; } = string.Empty;
    
    public string CityTo {  get; set; } = string.Empty;
    public DateOnly FlightDate { get; set; }

    public string FlightCompanyName { get; set; } = string.Empty;

    public int Quantity { get; set; }
    
    public Guid PriceCurrencyId { get; set; }
    
    public PriceCurrency? PriceCurrency { get; set; }
    
}

public class HotelBooking : BaseEntity
{
    public Guid TourId { get; set; }
    public Tour? Tour { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    
    public Guid PriceCurrencyId { get; set; }
    
    public PriceCurrency? PriceCurrency { get; set; }
}

public class ExtraService : BaseEntity
{
    public Guid TourId { get; set; }
    public Tour? Tour { get; set; }
    
    public Guid PriceCurrencyId { get; set; }
    
    public PriceCurrency? PriceCurrency { get; set; }
    
    public string Description { get; set; } = string.Empty;
}

