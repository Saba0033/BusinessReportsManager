using AutoMapper;
using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.Enums;
using BusinessReportsManager.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq;

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

    // Helper to extract user context
    // Roles:
    // - Accountant
    // - Supervisor
    // - Employee (any non-accountant, non-supervisor)
    private (string? role, string userId, bool isAccountant, bool isSupervisor, bool isEmployee)
        GetUserContext(ClaimsPrincipal user)
    {
        var role = user.FindFirst(ClaimTypes.Role)?.Value;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User identity not found.");

        bool isAccountant = role == "Accountant";
        bool isSupervisor = role == "Supervisor";
        bool isEmployee = !isAccountant && !isSupervisor;

        return (role, userId, isAccountant, isSupervisor, isEmployee);
    }

    // ---------------------------------------------------
    // CREATE FULL ORDER
    // Rule: Only Employee and Supervisor can create orders.
    // Accountant CANNOT create orders.
    // ---------------------------------------------------
    public async Task<OrderDto> CreateFullOrderAsync(OrderCreateDto dto, ClaimsPrincipal user)
    {
        var (_, userId, isAccountant, isSupervisor, isEmployee) = GetUserContext(user);

        if (!(isEmployee || isSupervisor))
            throw new UnauthorizedAccessException("Only employees or supervisors can create orders.");

        var email = user.FindFirst(ClaimTypes.Email)?.Value ?? "unknown@system.local";

        //
        // 1) FIND OR CREATE PARTY
        //
        OrderParty? party;

        if (dto.Party.Type.Equals("Person", StringComparison.OrdinalIgnoreCase))
        {
            // find existing person
            party = await _uow.OrderParties.Query()
                .OfType<PersonParty>()
                .FirstOrDefaultAsync(p =>
                    p.FirstName == dto.Party.FirstName &&
                    p.LastName == dto.Party.LastName &&
                    p.BirthDate == dto.Party.BirthDate);

            if (party == null)
            {
                party = _mapper.Map<PersonParty>(dto.Party);
                await _uow.OrderParties.AddAsync(party);
            }
        }
        else
        {
            // find existing company
            party = await _uow.OrderParties.Query()
                .OfType<CompanyParty>()
                .FirstOrDefaultAsync(p =>
                    p.RegistrationNumber == dto.Party.RegistrationNumber);

            if (party == null)
            {
                party = _mapper.Map<CompanyParty>(dto.Party);
                await _uow.OrderParties.AddAsync(party);
            }
        }

        //
        // 2) FIND OR CREATE SUPPLIER
        //
        Supplier? supplier = await _uow.Suppliers.Query()
            .FirstOrDefaultAsync(s =>
                s.Name == dto.Tour.Supplier.Name &&
                s.ContactEmail == dto.Tour.Supplier.ContactEmail);

        if (supplier == null)
        {
            supplier = _mapper.Map<Supplier>(dto.Tour.Supplier);
            await _uow.Suppliers.AddAsync(supplier);
        }

        //
        // 3) CREATE TOUR
        //
        var tour = _mapper.Map<Tour>(dto.Tour);
        tour.TourSupplier = supplier;

        await _uow.Tours.AddAsync(tour);

        //
        // 3.1 Passengers
        //
        foreach (var p in dto.Passengers)
        {
            var passenger = _mapper.Map<Passenger>(p);
            passenger.Tour = tour;
            await _uow.Passengers.AddAsync(passenger);
        }

        //
        // 3.2 Air Tickets
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
        // 3.3 Hotel Bookings
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
        // 3.4 Extra Services
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
        // 4) CREATE ORDER
        //
        var order = new Order
        {
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6]}",
            Source = dto.Source,
            SellPriceInGel = dto.SellPriceInGel,
            Status = OrderStatus.Open,
            OrderParty = party,
            Tour = tour,
            CreatedById = userId,
            CreatedByEmail = email
        };

        await _uow.Orders.AddAsync(order);

        //
        // 5) PAYMENTS (always new)
        //
        foreach (var pay in dto.Payments)
        {
            var price = new PriceCurrency
            {
                Amount = pay.Amount,
                Currency = pay.Currency,
                ExchangeRateToGel = 1
            };

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
    // Rule:
    // - If FINALIZED → only Accountant OR Supervisor can edit.
    // - If OPEN → Employee can edit ONLY his own order, Accountant can edit any,
    //             Supervisor can edit everything.
    // ---------------------------------------------------
    public async Task<OrderDto?> EditOrderAsync(Guid orderId, OrderEditDto dto, ClaimsPrincipal user)
    {
        var (_, userId, isAccountant, isSupervisor, isEmployee) = GetUserContext(user);

        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order == null)
            return null;

        bool isCreator = order.CreatedById == userId;
        bool isFinalized = order.Status == OrderStatus.Finalized;

        if (isFinalized)
        {
            // Finalized: accountant or supervisor only
            if (!(isAccountant || isSupervisor))
                throw new UnauthorizedAccessException("Only accountant or supervisor can edit finalized orders.");
        }
        else
        {
            // Open:
            // - Supervisor → always
            // - Accountant → always
            // - Employee → only own order
            if (!(isSupervisor || isAccountant || (isEmployee && isCreator)))
                throw new UnauthorizedAccessException("You are not allowed to edit this order.");
        }

        // Modify allowed fields
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
    // Rule: Accountant OR Supervisor can add payments.
    // ---------------------------------------------------
    public async Task<PaymentDto?> AddPaymentAsync(Guid orderId, PaymentCreateDto dto, ClaimsPrincipal user)
    {
        var (_, _, isAccountant, isSupervisor, _) = GetUserContext(user);

        if (!(isAccountant || isSupervisor))
            throw new UnauthorizedAccessException("Only accountant or supervisor can add payments.");

        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order == null) return null;

        var price = new PriceCurrency
        {
            Amount = dto.Amount,
            Currency = dto.Currency,
            ExchangeRateToGel = 1 // default
        };

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
    // Rule: Accountant OR Supervisor can remove payments.
    // ---------------------------------------------------
    public async Task<bool> RemovePaymentAsync(Guid paymentId, ClaimsPrincipal user)
    {
        var (_, _, isAccountant, isSupervisor, _) = GetUserContext(user);

        if (!(isAccountant || isSupervisor))
            throw new UnauthorizedAccessException("Only accountant or supervisor can remove payments.");

        var payment = await _uow.Payments.GetByIdAsync(paymentId);
        if (payment == null) return false;

        await _uow.Payments.RemoveAsync(payment);
        await _uow.SaveChangesAsync();
        return true;
    }

    // ---------------------------------------------------
    // CHANGE STATUS
    // Rule: Accountant OR Supervisor
    // (matches what you confirmed as "yes")
    // ---------------------------------------------------
    public async Task<bool> ChangeStatusAsync(Guid orderId, OrderStatus newStatus, ClaimsPrincipal user)
    {
        var (_, _, isAccountant, isSupervisor, _) = GetUserContext(user);

        if (!(isAccountant || isSupervisor))
            throw new UnauthorizedAccessException("Only accountant or supervisor can change order status.");

        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order == null) return false;

        order.Status = newStatus;

        await _uow.Orders.UpdateAsync(order);
        await _uow.SaveChangesAsync();

        return true;
    }

    // ---------------------------------------------------
    // DELETE ORDER
    // Rule: Accountant OR Supervisor
    // ---------------------------------------------------
    public async Task<bool> DeleteOrderAsync(Guid orderId, ClaimsPrincipal user)
    {
        var (_, _, isAccountant, isSupervisor, _) = GetUserContext(user);

        if (!(isAccountant || isSupervisor))
            throw new UnauthorizedAccessException("Only accountant or supervisor can delete orders.");

        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order == null) return false;

        await _uow.Orders.RemoveAsync(order);
        await _uow.SaveChangesAsync();

        return true;
    }

    // ---------------------------------------------------
    // GET ALL
    // Your rule: "supervisor only" (for seeing all),
    // regular user only sees his orders.
    //
    // Implementation:
    // - Supervisor → all orders
    // - Everyone else → only their own (CreatedById == userId)
    // ---------------------------------------------------
    public async Task<List<OrderDto>> GetAllAsync(ClaimsPrincipal user)
    {
        var (_, userId, _, isSupervisor, _) = GetUserContext(user);
        IQueryable<Order> query = _uow.Orders.Query();
        query = _uow.Orders.Query()
            .Include(o => o.OrderParty)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.Passengers)
            .Include(o => o.Tour).ThenInclude(t => t.AirTickets).ThenInclude(a => a.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.HotelBookings).ThenInclude(h => h.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.ExtraServices).ThenInclude(e => e.PriceCurrency);

        if (!isSupervisor)
        {
            query = query.Where(o => o.CreatedById == userId);
        }

        var orders = await query.ToListAsync();

        return _mapper.Map<List<OrderDto>>(orders);
    }

    // ---------------------------------------------------
    // GET BY ID
    // Rule:
    // - Supervisor → can view any order
    // - Regular user → only his own orders
    // ---------------------------------------------------
    public async Task<OrderDto?> GetByIdAsync(Guid id, ClaimsPrincipal user)
    {
        var (_, userId, _, isSupervisor, _) = GetUserContext(user);

        var query = _uow.Orders.Query()
            .Include(o => o.OrderParty)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.Passengers)
            .Include(o => o.Tour).ThenInclude(t => t.AirTickets).ThenInclude(a => a.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.HotelBookings).ThenInclude(h => h.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.ExtraServices).ThenInclude(e => e.PriceCurrency)
            .Where(o => o.Id == id);

        var order = await query.FirstOrDefaultAsync();
        if (order == null) return null;

        if (!isSupervisor && order.CreatedById != userId)
            throw new UnauthorizedAccessException("You are not allowed to view this order.");

        return _mapper.Map<OrderDto>(order);
    }

    // ---------------------------------------------------
    // GET BY STATUS
    // Rule:
    // - Supervisor → all with that status
    // - Regular user → only own orders with that status
    // ---------------------------------------------------
    public async Task<List<OrderDto>> GetByStatusAsync(OrderStatus status, ClaimsPrincipal user)
    {
        var (_, userId, _, isSupervisor, _) = GetUserContext(user);

        IQueryable<Order> query = _uow.Orders.Query();
        query = _uow.Orders.Query(o => o.Status == status)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour);

        if (!isSupervisor)
        {
            query = query.Where(o => o.CreatedById == userId);
        }

        var orders = await query.ToListAsync();

        return _mapper.Map<List<OrderDto>>(orders);
    }

    // ---------------------------------------------------
    // GET BY PARTY
    // Rule:
    // - Supervisor → all for that party
    // - Regular user → only own orders for that party
    // ---------------------------------------------------
    public async Task<List<OrderDto>> GetByPartyAsync(Guid partyId, ClaimsPrincipal user)
    {
        var (_, userId, _, isSupervisor, _) = GetUserContext(user);

        IQueryable<Order> query = _uow.Orders.Query();
        query = _uow.Orders.Query(o => o.OrderPartyId == partyId)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour);

        if (!isSupervisor)
        {
            query = query.Where(o => o.CreatedById == userId);
        }

        var orders = await query.ToListAsync();

        return _mapper.Map<List<OrderDto>>(orders);
    }

    // ---------------------------------------------------
    // GET BY DATE RANGE
    // Rule:
    // - Supervisor → all in range
    // - Regular user → only own in that range
    // ---------------------------------------------------
    public async Task<List<OrderDto>> GetByDateRangeAsync(DateTime start, DateTime end, ClaimsPrincipal user)
    {
        var (_, userId, _, isSupervisor, _) = GetUserContext(user);

        IQueryable<Order> query = _uow.Orders.Query();
        query = _uow.Orders.Query(o =>
                o.CreatedAtUtc >= start && o.CreatedAtUtc <= end)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour);

        if (!isSupervisor)
        {
            query = query.Where(o => o.CreatedById == userId);
        }

        var orders = await query.ToListAsync();

        return _mapper.Map<List<OrderDto>>(orders);
    }

    // ---------------------------------------------------
    // TOTAL PAID (no user needed here usually)
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
