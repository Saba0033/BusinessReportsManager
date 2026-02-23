using System.Threading.Tasks;
using BusinessReportsManager.Domain.Entities;

namespace BusinessReportsManager.Domain.Interfaces;

public interface IUnitOfWork
{
    IGenericRepository<Order> Orders { get; }
    IGenericRepository<Tour> Tours { get; }
    IGenericRepository<Passenger> Passengers { get; }
    IGenericRepository<Payment> Payments { get; }
    IGenericRepository<PriceCurrency> PriceCurrencies { get; }
    IGenericRepository<Supplier> Suppliers { get; }
    IGenericRepository<OrderParty> OrderParties { get; }
    
    IGenericRepository<AirTicket> AirTickets { get; }
    
    IGenericRepository<HotelBooking> HotelBookings { get; }
    IGenericRepository<ExtraService> ExtraServices { get; }
    IGenericRepository<CustomerBankRequisites> CustomerBankRequisites { get; }


    Task<int> SaveChangesAsync();
}