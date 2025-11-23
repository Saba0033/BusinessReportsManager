using AutoMapper;
using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.Enums;
using BusinessReportsManager.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessReportsManager.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    // ---------------------------------------------------
    // CREATE FULL ORDER
    // ---------------------------------------------------
    public async Task<OrderDto> CreateFullOrderAsync(OrderCreateDto dto)
    {
        //
        // 1) CREATE PARTY
        //
        OrderParty party = dto.Party.Type.Equals("Company", StringComparison.OrdinalIgnoreCase)
            ? _mapper.Map<CompanyParty>(dto.Party)
            : _mapper.Map<PersonParty>(dto.Party);

        await _uow.OrderParties.AddAsync(party);

        //
        // 2) CREATE TOUR
        //
        var tour = _mapper.Map<Tour>(dto.Tour);

        // Supplier
        var supplier = _mapper.Map<Supplier>(dto.Tour.Supplier);
        tour.TourSupplier = supplier;

        await _uow.Tours.AddAsync(tour);

        //
        // 2.1) Passengers
        //
        foreach (var p in dto.Passengers)
        {
            var passenger = _mapper.Map<Passenger>(p);
            passenger.Tour = tour;
            await _uow.Passengers.AddAsync(passenger);
        }

        //
        // 2.2) Air tickets
        //
        foreach (var t in dto.Tour.AirTickets)
        {
            var ticket = _mapper.Map<AirTicket>(t);
            ticket.Tour = tour;

            var price = _mapper.Map<PriceCurrency>(t.Price);
            ticket.PriceCurrency = price;

            await _uow.PriceCurrencies.AddAsync(price);
            await _uow.AirTickets.AddAsync(ticket);
        }

        //
        // 2.3) Hotel bookings
        //
        foreach (var h in dto.Tour.HotelBookings)
        {
            var hotel = _mapper.Map<HotelBooking>(h);
            hotel.Tour = tour;

            var price = _mapper.Map<PriceCurrency>(h.Price);
            hotel.PriceCurrency = price;

            await _uow.PriceCurrencies.AddAsync(price);
            await _uow.HotelBookings.AddAsync(hotel);
        }

        //
        // 2.4) Extra services
        //
        foreach (var e in dto.Tour.ExtraServices)
        {
            var extra = _mapper.Map<ExtraService>(e);
            extra.Tour = tour;

            var price = _mapper.Map<PriceCurrency>(e.Price);
            extra.PriceCurrency = price;

            await _uow.PriceCurrencies.AddAsync(price);
            await _uow.ExtraServices.AddAsync(extra);
        }

        //
        // 3) CREATE ORDER
        //
        var order = new Order
        {
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6]}",
            Source = dto.Source,
            SellPriceInGel = dto.SellPriceInGel,
            Status = OrderStatus.Open,
            OrderParty = party,
            Tour = tour
        };

        await _uow.Orders.AddAsync(order);

        //
        // 4) INIT PAYMENTS
        //
        foreach (var pay in dto.Payments)
        {
            var price = _mapper.Map<PriceCurrency>(new PriceCurrencyCreateDto
            {
                Amount = pay.Amount,
                Currency = pay.Currency,
                ExchangeRateToGel = 1 // or dto value if required
            });

            await _uow.PriceCurrencies.AddAsync(price);

            var payment = new Payment
            {
                Order = order,
                PriceCurrency = price,
                BankName = pay.BankName,
                Reference = pay.Reference,
                PaidDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            await _uow.Payments.AddAsync(payment);
        }

        await _uow.SaveChangesAsync();

        return _mapper.Map<OrderDto>(order);
    }

    // ---------------------------------------------------
    // EDIT ORDER
    // ---------------------------------------------------
    public async Task<OrderDto?> EditOrderAsync(Guid orderId, OrderEditDto dto)
    {
        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order == null) return null;

        if (dto.SellPriceInGel.HasValue)
            order.SellPriceInGel = dto.SellPriceInGel.Value;

        if (!string.IsNullOrWhiteSpace(dto.Source))
            order.Source = dto.Source;

        await _uow.Orders.UpdateAsync(order);
        await _uow.SaveChangesAsync();

        return _mapper.Map<OrderDto>(order);
    }

    // ---------------------------------------------------
    // ADD PAYMENT
    // ---------------------------------------------------
    public async Task<PaymentDto?> AddPaymentAsync(Guid orderId, PaymentCreateDto dto)
    {
        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order == null) return null;

        var price = _mapper.Map<PriceCurrency>(new PriceCurrencyCreateDto
        {
            Amount = dto.Amount,
            Currency = dto.Currency,
            ExchangeRateToGel = 1 // default
        });

        await _uow.PriceCurrencies.AddAsync(price);

        var payment = new Payment
        {
            OrderId = orderId,
            PriceCurrency = price,
            BankName = dto.BankName,
            Reference = dto.Reference,
            PaidDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        await _uow.Payments.AddAsync(payment);
        await _uow.SaveChangesAsync();

        return _mapper.Map<PaymentDto>(payment);
    }

    // ---------------------------------------------------
    // REMOVE PAYMENT
    // ---------------------------------------------------
    public async Task<bool> RemovePaymentAsync(Guid paymentId)
    {
        var payment = await _uow.Payments.GetByIdAsync(paymentId);
        if (payment == null) return false;

        await _uow.Payments.RemoveAsync(payment);
        await _uow.SaveChangesAsync();
        return true;
    }

    // ---------------------------------------------------
    // CHANGE STATUS
    // ---------------------------------------------------
    public async Task<bool> ChangeStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order == null) return false;

        order.Status = newStatus;

        await _uow.Orders.UpdateAsync(order);
        await _uow.SaveChangesAsync();

        return true;
    }

    // ---------------------------------------------------
    // DELETE ORDER
    // ---------------------------------------------------
    public async Task<bool> DeleteOrderAsync(Guid orderId)
    {
        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order == null) return false;

        await _uow.Orders.RemoveAsync(order);
        await _uow.SaveChangesAsync();

        return true;
    }

    // ---------------------------------------------------
    // GET ALL
    // ---------------------------------------------------
    public async Task<List<OrderDto>> GetAllAsync()
    {
        var orders = await _uow.Orders.Query()
            .Include(o => o.OrderParty)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.Passengers)
            .Include(o => o.Tour).ThenInclude(t => t.AirTickets).ThenInclude(a => a.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.HotelBookings).ThenInclude(h => h.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.ExtraServices).ThenInclude(e => e.PriceCurrency)
            .ToListAsync();

        return _mapper.Map<List<OrderDto>>(orders);
    }

    // ---------------------------------------------------
    // GET BY ID
    // ---------------------------------------------------
    public async Task<OrderDto?> GetByIdAsync(Guid id)
    {
        var order = await _uow.Orders.Query()
            .Include(o => o.OrderParty)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.Passengers)
            .Include(o => o.Tour).ThenInclude(t => t.AirTickets).ThenInclude(a => a.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.HotelBookings).ThenInclude(h => h.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.ExtraServices).ThenInclude(e => e.PriceCurrency)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order == null ? null : _mapper.Map<OrderDto>(order);
    }

    // ---------------------------------------------------
    // GET BY STATUS
    // ---------------------------------------------------
    public async Task<List<OrderDto>> GetByStatusAsync(OrderStatus status)
    {
        var orders = await _uow.Orders.Query(o => o.Status == status)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour)
            .ToListAsync();

        return _mapper.Map<List<OrderDto>>(orders);
    }

    // ---------------------------------------------------
    // GET BY PARTY
    // ---------------------------------------------------
    public async Task<List<OrderDto>> GetByPartyAsync(Guid partyId)
    {
        var orders = await _uow.Orders.Query(o => o.OrderPartyId == partyId)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour)
            .ToListAsync();

        return _mapper.Map<List<OrderDto>>(orders);
    }

    // ---------------------------------------------------
    // GET BY DATE RANGE
    // ---------------------------------------------------
    public async Task<List<OrderDto>> GetByDateRangeAsync(DateTime start, DateTime end)
    {
        var orders = await _uow.Orders.Query(o =>
                o.CreatedAtUtc >= start && o.CreatedAtUtc <= end)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour)
            .ToListAsync();

        return _mapper.Map<List<OrderDto>>(orders);
    }

    // ---------------------------------------------------
    // TOTAL PAID
    // ---------------------------------------------------
    public async Task<decimal> GetTotalPaidAsync(Guid orderId)
    {
        return await _uow.Payments.Query(p => p.OrderId == orderId)
            .Join(_uow.PriceCurrencies.Query(),
                  p => p.PriceCurrencyId,
                  c => c.Id,
                  (p, c) => c.Amount)
            .SumAsync();
    }
}
