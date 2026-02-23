using System.Threading.Tasks;
using BusinessReportsManager.Domain.Interfaces;
using BusinessReportsManager.Domain.Entities;

namespace BusinessReportsManager.Infrastructure.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;

        Orders = new GenericRepository<Order>(context);
        Tours = new GenericRepository<Tour>(context);
        Passengers = new GenericRepository<Passenger>(context);
        Payments = new GenericRepository<Payment>(context);
        PriceCurrencies = new GenericRepository<PriceCurrency>(context);
        Suppliers = new GenericRepository<Supplier>(context);
        OrderParties = new GenericRepository<OrderParty>(context);
        AirTickets = new GenericRepository<AirTicket>(context);
        HotelBookings = new GenericRepository<HotelBooking>(context);
        ExtraServices = new GenericRepository<ExtraService>(context);
        CustomerBankRequisites = new GenericRepository<CustomerBankRequisites>(context);

    }

    public IGenericRepository<Order> Orders { get; }
    public IGenericRepository<Tour> Tours { get; }
    public IGenericRepository<Passenger> Passengers { get; }
    public IGenericRepository<Payment> Payments { get; }
    public IGenericRepository<PriceCurrency> PriceCurrencies { get; }
    public IGenericRepository<Supplier> Suppliers { get; }
    public IGenericRepository<OrderParty> OrderParties { get; }
    public IGenericRepository<AirTicket> AirTickets { get; }
    public IGenericRepository<HotelBooking> HotelBookings { get; }
    public IGenericRepository<ExtraService> ExtraServices { get; }
    public IGenericRepository<CustomerBankRequisites> CustomerBankRequisites { get; }




    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}