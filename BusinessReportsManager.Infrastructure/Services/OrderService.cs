using AutoMapper;
using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs.Order;
using BusinessReportsManager.Application.DTOs.OrderParty;
using BusinessReportsManager.Application.DTOs.Payment;
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
        // 1) PARTY
        var party = CreatePartyFromDto(dto.Party);
        await _uow.OrderParties.AddAsync(party);

        // 2) TOUR + nested graph
        var tour = await CreateTourGraphAsync(dto);

        // 3) ORDER
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

        // 4) PAYMENTS (full add)
        await RebuildPaymentsAsync(order, dto.Payments);

        await _uow.SaveChangesAsync();
        return _mapper.Map<OrderDto>(order);
    }

    // ---------------------------------------------------
    // EDIT ORDER (FULL REPLACE OF NESTED GRAPH)
    // ---------------------------------------------------
    public async Task<OrderDto?> EditOrderAsync(Guid orderId, OrderEditDto dto)
    {
        var order = await LoadOrderGraphAsync(orderId);
        if (order == null) return null;

        // 1) SIMPLE ORDER FIELDS
        order.Source = dto.Source;
        order.SellPriceInGel = dto.SellPriceInGel;

        // 2) PARTY
        await UpdatePartyAsync(order, dto.Party);

        // 3) TOUR ROOT
        _mapper.Map(dto.Tour, order.Tour);

        // 4) TOUR SUBCOLLECTIONS (full replace)
        await RebuildPassengersAsync(order, dto);
        await RebuildAirTicketsAsync(order, dto);
        await RebuildHotelBookingsAsync(order, dto);
        await RebuildExtraServicesAsync(order, dto);

        // 5) PAYMENTS (full replace)
        await RebuildPaymentsAsync(order, dto.Payments);

        await _uow.Orders.UpdateAsync(order);
        await _uow.SaveChangesAsync();

        return _mapper.Map<OrderDto>(order);
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
    // QUERIES
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

    public async Task<List<OrderDto>> GetByStatusAsync(OrderStatus status)
    {
        var orders = await _uow.Orders.Query(o => o.Status == status)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour)
            .ToListAsync();

        return _mapper.Map<List<OrderDto>>(orders);
    }

    public async Task<List<OrderDto>> GetByPartyAsync(Guid partyId)
    {
        var orders = await _uow.Orders.Query(o => o.OrderPartyId == partyId)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour)
            .ToListAsync();

        return _mapper.Map<List<OrderDto>>(orders);
    }

    public async Task<List<OrderDto>> GetByDateRangeAsync(DateTime start, DateTime end)
    {
        var orders = await _uow.Orders.Query(o =>
                o.CreatedAtUtc >= start && o.CreatedAtUtc <= end)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour)
            .ToListAsync();

        return _mapper.Map<List<OrderDto>>(orders);
    }

    // ===================================================
    // PRIVATE HELPERS
    // ===================================================

    private static OrderParty CreatePartyFromDto(PartyCreateDto dto)
    {
        return dto.Type.Equals("Company", StringComparison.OrdinalIgnoreCase)
            ? new CompanyParty
            {
                Email = dto.Email,
                Phone = dto.Phone,
                CompanyName = dto.CompanyName,
                RegistrationNumber = dto.RegistrationNumber,
                ContactPerson = dto.ContactPerson
            }
            : new PersonParty
            {
                Email = dto.Email,
                Phone = dto.Phone,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate
            };
    }

    private async Task<Tour> CreateTourGraphAsync(OrderCreateDto dto)
    {
        var tour = _mapper.Map<Tour>(dto.Tour);

        // Supplier
        var supplier = _mapper.Map<Supplier>(dto.Tour.Supplier);
        tour.TourSupplier = supplier;

        await _uow.Tours.AddAsync(tour);

        // Passengers
        foreach (var p in dto.Passengers)
        {
            var passenger = _mapper.Map<Passenger>(p);
            passenger.Tour = tour;
            await _uow.Passengers.AddAsync(passenger);
        }

        // Air tickets
        foreach (var t in dto.Tour.AirTickets)
        {
            var ticket = _mapper.Map<AirTicket>(t);
            ticket.Tour = tour;

            var price = _mapper.Map<PriceCurrency>(t.Price);
            ticket.PriceCurrency = price;

            await _uow.PriceCurrencies.AddAsync(price);
            await _uow.AirTickets.AddAsync(ticket);
        }

        // Hotel bookings
        foreach (var h in dto.Tour.HotelBookings)
        {
            var hotel = _mapper.Map<HotelBooking>(h);
            hotel.Tour = tour;

            var price = _mapper.Map<PriceCurrency>(h.Price);
            hotel.PriceCurrency = price;

            await _uow.PriceCurrencies.AddAsync(price);
            await _uow.HotelBookings.AddAsync(hotel);
        }

        // Extra services
        foreach (var e in dto.Tour.ExtraServices)
        {
            var extra = _mapper.Map<ExtraService>(e);
            extra.Tour = tour;

            var price = _mapper.Map<PriceCurrency>(e.Price);
            extra.PriceCurrency = price;

            await _uow.PriceCurrencies.AddAsync(price);
            await _uow.ExtraServices.AddAsync(extra);
        }

        return tour;
    }

    private async Task<Order?> LoadOrderGraphAsync(Guid orderId)
    {
        return await _uow.Orders.Query()
            .Include(o => o.OrderParty)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.Passengers)
            .Include(o => o.Tour).ThenInclude(t => t.AirTickets).ThenInclude(a => a.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.HotelBookings).ThenInclude(h => h.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.ExtraServices).ThenInclude(e => e.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.TourSupplier)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    private async Task UpdatePartyAsync(Order order, PartyCreateDto dto)
    {
        var isCompany = dto.Type.Equals("Company", StringComparison.OrdinalIgnoreCase);

        if (isCompany && order.OrderParty is CompanyParty companyParty)
        {
            _mapper.Map(dto, companyParty);
        }
        else if (!isCompany && order.OrderParty is PersonParty personParty)
        {
            _mapper.Map(dto, personParty);
        }
        else
        {
            var oldParty = order.OrderParty;
            var newParty = CreatePartyFromDto(dto);

            order.OrderParty = newParty;

            await _uow.OrderParties.AddAsync(newParty);
            if (oldParty != null)
                await _uow.OrderParties.RemoveAsync(oldParty);
        }
    }

    private async Task RebuildPassengersAsync(Order order, OrderEditDto dto)
    {
        foreach (var p in order.Tour!.Passengers.ToList())
            await _uow.Passengers.RemoveAsync(p);

        order.Tour.Passengers.Clear();

        foreach (var pDto in dto.Passengers)
        {
            var passenger = _mapper.Map<Passenger>(pDto);
            passenger.Tour = order.Tour;
            await _uow.Passengers.AddAsync(passenger);
        }
    }

    private async Task RebuildAirTicketsAsync(Order order, OrderEditDto dto)
    {
        foreach (var t in order.Tour!.AirTickets.ToList())
        {
            if (t.PriceCurrency != null)
                await _uow.PriceCurrencies.RemoveAsync(t.PriceCurrency);

            await _uow.AirTickets.RemoveAsync(t);
        }
        order.Tour.AirTickets.Clear();

        foreach (var tDto in dto.Tour.AirTickets)
        {
            var ticket = _mapper.Map<AirTicket>(tDto);
            ticket.Tour = order.Tour;

            var price = _mapper.Map<PriceCurrency>(tDto.Price);
            ticket.PriceCurrency = price;

            await _uow.PriceCurrencies.AddAsync(price);
            await _uow.AirTickets.AddAsync(ticket);
        }
    }

    private async Task RebuildHotelBookingsAsync(Order order, OrderEditDto dto)
    {
        foreach (var h in order.Tour!.HotelBookings.ToList())
        {
            if (h.PriceCurrency != null)
                await _uow.PriceCurrencies.RemoveAsync(h.PriceCurrency);

            await _uow.HotelBookings.RemoveAsync(h);
        }
        order.Tour.HotelBookings.Clear();

        foreach (var hDto in dto.Tour.HotelBookings)
        {
            var hotel = _mapper.Map<HotelBooking>(hDto);
            hotel.Tour = order.Tour;

            var price = _mapper.Map<PriceCurrency>(hDto.Price);
            hotel.PriceCurrency = price;

            await _uow.PriceCurrencies.AddAsync(price);
            await _uow.HotelBookings.AddAsync(hotel);
        }
    }

    private async Task RebuildExtraServicesAsync(Order order, OrderEditDto dto)
    {
        foreach (var e in order.Tour!.ExtraServices.ToList())
        {
            if (e.PriceCurrency != null)
                await _uow.PriceCurrencies.RemoveAsync(e.PriceCurrency);

            await _uow.ExtraServices.RemoveAsync(e);
        }
        order.Tour.ExtraServices.Clear();

        foreach (var eDto in dto.Tour.ExtraServices)
        {
            var extra = _mapper.Map<ExtraService>(eDto);
            extra.Tour = order.Tour;

            var price = _mapper.Map<PriceCurrency>(eDto.Price);
            extra.PriceCurrency = price;

            await _uow.PriceCurrencies.AddAsync(price);
            await _uow.ExtraServices.AddAsync(extra);
        }
    }

    private async Task RebuildPaymentsAsync(Order order, IEnumerable<PaymentCreateDto> paymentDtos)
    {
        // remove existing
        foreach (var existing in order.Payments.ToList())
        {
            if (existing.PriceCurrency != null)
                await _uow.PriceCurrencies.RemoveAsync(existing.PriceCurrency);

            await _uow.Payments.RemoveAsync(existing);
        }
        order.Payments.Clear();

        // add new
        foreach (var dto in paymentDtos)
        {
            var price = new PriceCurrency
            {
                Amount = dto.Price.Amount,
                Currency = dto.Price.Currency,
                ExchangeRateToGel = dto.Price.ExchangeRateToGel,
                EffectiveDate = dto.Price.EffectiveDate
            };

            await _uow.PriceCurrencies.AddAsync(price);

            var payment = new Payment
            {
                Order = order,
                PriceCurrency = price,
                BankName = dto.BankName,
                PaidDate = dto.PaidDate
            };

            await _uow.Payments.AddAsync(payment);
        }
    }
}
