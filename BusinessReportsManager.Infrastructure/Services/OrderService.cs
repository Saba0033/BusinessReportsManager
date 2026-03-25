using AutoMapper;
using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Application.DTOs.Order;
using BusinessReportsManager.Application.DTOs.OrderParty;
using BusinessReportsManager.Application.DTOs.Payment;
using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.Enums;
using BusinessReportsManager.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BusinessReportsManager.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;


    public OrderService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor contextAccessor)
    {
        _uow = uow;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
    }

    // ---------------------------------------------------
    // CREATE FULL ORDER
    // ---------------------------------------------------
    public async Task<OrderDto> CreateFullOrderAsync(OrderCreateDto dto)
    {
        var user = _contextAccessor.HttpContext?.User;

        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = user?.FindFirst(ClaimTypes.Email)?.Value;

        if (userId == null || email == null)
            throw new Exception("Unable to read user identity from JWT.");

        CustomerBankRequisites? req = null;

        if (dto.CustomerBankRequisites != null)
        {
            req = _mapper.Map<CustomerBankRequisites>(dto.CustomerBankRequisites);
            await _uow.CustomerBankRequisites.AddAsync(req);
        }

        // 1) PARTY
        var party = await GetOrCreatePersonPartyAsync(dto.Party);

        // 2) TOUR
        var tour = await CreateTourGraphAsync(dto);

        // 3) ORDER
        var nextOrderNumber = await GetNextOrderNumberAsync();

        var order = new Order
        {
            OrderNumber = nextOrderNumber,
            Source = dto.Source,
            TourType = dto.TourType,
            ManagerName = ResolveManagerName(user, dto.ManagerName),
            SellPriceInGel = dto.SellPriceInGel,
            TotalExpenseInGel = dto.TotalExpenseInGel,
            TicketNet = dto.TicketNet,
            TicketSupplier = dto.TicketSupplier,
            HotelNet = dto.HotelNet,
            HotelSupplier = dto.HotelSupplier,
            TransferNet = dto.TransferNet,
            TransferSupplier = dto.TransferSupplier,
            InsuranceNet = dto.InsuranceNet,
            InsuranceSupplier = dto.InsuranceSupplier,
            OtherServiceNet = dto.OtherServiceNet,
            OtherServiceSupplier = dto.OtherServiceSupplier,
            Status = OrderStatus.Open,
            OrderPartyId = party.Id,
            Tour = tour,
            CreatedById = Guid.Parse(userId),
            CreatedByEmail = email,
            CustomerBankRequisites = req
        };

        await _uow.Orders.AddAsync(order);
        await _uow.SaveChangesAsync();

        var createdOrder = await LoadOrderGraphAsync(order.Id);
        if (createdOrder == null)
            throw new Exception("Created order could not be reloaded.");

        return _mapper.Map<OrderDto>(createdOrder);
    }



    // ---------------------------------------------------
    // EDIT ORDER (FULL REPLACE OF NESTED GRAPH)
    // ---------------------------------------------------
    public async Task<OrderDto?> EditOrderAsync(Guid orderId, OrderEditDto dto)
    {
        var order = await LoadOrderGraphAsync(orderId);
        if (order == null) return null;

        var user = _contextAccessor.HttpContext?.User;

        // 1) SIMPLE ORDER FIELDS
        order.Source = dto.Source;
        order.TourType = dto.TourType;
        order.ManagerName = ResolveManagerName(user, dto.ManagerName);
        order.SellPriceInGel = dto.SellPriceInGel;
        order.TotalExpenseInGel = dto.TotalExpenseInGel;
        order.TicketNet = dto.TicketNet;
        order.TicketSupplier = dto.TicketSupplier;
        order.HotelNet = dto.HotelNet;
        order.HotelSupplier = dto.HotelSupplier;
        order.TransferNet = dto.TransferNet;
        order.TransferSupplier = dto.TransferSupplier;
        order.InsuranceNet = dto.InsuranceNet;
        order.InsuranceSupplier = dto.InsuranceSupplier;
        order.OtherServiceNet = dto.OtherServiceNet;
        order.OtherServiceSupplier = dto.OtherServiceSupplier;

        // 2) PARTY (person-only)
        await UpdatePartyAsync(order, dto.Party);

        // 3) TOUR ROOT
        order.Tour.Name = dto.Destination;
        order.Tour.StartDate = dto.StartDate;
        order.Tour.EndDate = dto.EndDate;
        order.Tour.PassengerCount = dto.PassengerCount;

        // 4) TOUR SUBCOLLECTIONS (full replace)
        await RebuildPassengersAsync(order, dto);
        await RebuildAirTicketsAsync(order, dto);
        await RebuildHotelBookingsAsync(order, dto);

        // ExtraServices NOT edited here anymore (separate endpoint later)
        // Payments NOT edited here anymore (separate endpoint later)

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
            .Include(o => o.CustomerBankRequisites)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.Passengers)
            .Include(o => o.Tour).ThenInclude(t => t.AirTickets).ThenInclude(a => a.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.HotelBookings).ThenInclude(h => h.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.ExtraServices).ThenInclude(e => e.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.TourSupplier)
            .ToListAsync();

        return _mapper.Map<List<OrderDto>>(orders);
    }

    public async Task<OrderDto?> GetByIdAsync(Guid id)
    {
        var order = await _uow.Orders.Query()
            .Include(o => o.OrderParty)
            .Include(o => o.CustomerBankRequisites)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.Passengers)
            .Include(o => o.Tour).ThenInclude(t => t.AirTickets).ThenInclude(a => a.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.HotelBookings).ThenInclude(h => h.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.ExtraServices).ThenInclude(e => e.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.TourSupplier)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order == null ? null : _mapper.Map<OrderDto>(order);
    }

    public async Task<List<OrderDto>> GetByStatusAsync(OrderStatus status)
    {
        var orders = await _uow.Orders.Query(o => o.Status == status)
            .Include(o => o.OrderParty)
            .Include(o => o.CustomerBankRequisites)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.Passengers)
            .Include(o => o.Tour).ThenInclude(t => t.AirTickets).ThenInclude(a => a.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.HotelBookings).ThenInclude(h => h.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.ExtraServices).ThenInclude(e => e.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.TourSupplier)
            .ToListAsync();

        return _mapper.Map<List<OrderDto>>(orders);
    }

    public async Task<List<OrderDto>> GetByPartyAsync(Guid partyId)
    {
        var orders = await _uow.Orders.Query(o => o.OrderPartyId == partyId)
            .Include(o => o.OrderParty)
            .Include(o => o.CustomerBankRequisites)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.Passengers)
            .Include(o => o.Tour).ThenInclude(t => t.AirTickets).ThenInclude(a => a.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.HotelBookings).ThenInclude(h => h.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.ExtraServices).ThenInclude(e => e.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.TourSupplier)
            .ToListAsync();

        return _mapper.Map<List<OrderDto>>(orders);
    }

    public async Task<List<OrderDto>> GetByDateRangeAsync(DateTime start, DateTime end)
    {
        var orders = await _uow.Orders.Query(o =>
                o.CreatedAtUtc >= start && o.CreatedAtUtc <= end)
            .Include(o => o.OrderParty)
            .Include(o => o.CustomerBankRequisites)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.Passengers)
            .Include(o => o.Tour).ThenInclude(t => t.AirTickets).ThenInclude(a => a.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.HotelBookings).ThenInclude(h => h.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.ExtraServices).ThenInclude(e => e.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.TourSupplier)
            .ToListAsync();

        return _mapper.Map<List<OrderDto>>(orders);
    }

    public async Task<bool> UpdateAccountingCommentAsync(Guid orderId, string? comment)
    {
        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order == null) return false;

        var user = _contextAccessor.HttpContext?.User;

        var userIdStr = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = user?.FindFirst(ClaimTypes.Email)?.Value;

        order.AccountingComment = comment;
        order.AccountingCommentUpdatedAtUtc = DateTime.UtcNow;
        order.AccountingCommentUpdatedByEmail = email;

        if (Guid.TryParse(userIdStr, out var userId))
            order.AccountingCommentUpdatedById = userId;

        await _uow.Orders.UpdateAsync(order);
        await _uow.SaveChangesAsync();

        return true;
    }

    private async Task<int> GetNextOrderNumberAsync()
    {
        var max = await _uow.Orders.Query()
            .Select(o => (int?)o.OrderNumber)
            .MaxAsync() ?? 0;
        return max + 1;
    }

    private static string? ResolveManagerName(ClaimsPrincipal? user, string? dtoFallback)
    {
        var fromJwt = user?.FindFirst("username")?.Value
            ?? user?.FindFirst(ClaimTypes.Name)?.Value;
        return string.IsNullOrWhiteSpace(fromJwt) ? dtoFallback : fromJwt;
    }

    // ===================================================
    // PRIVATE HELPERS
    // ===================================================

    private async Task<PersonParty> GetOrCreatePersonPartyAsync(PartyCreateDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var normalizedEmail = NormalizeEmail(dto.Email);
        var normalizedPn = NormalizePersonalNumber(dto.PersonalNumber);

        // 0) Best match: explicit existing party Id from saved customers
        if (dto.Id.HasValue && dto.Id.Value != Guid.Empty)
        {
            var existingById = await _uow.OrderParties.Query()
                .OfType<PersonParty>()
                .FirstOrDefaultAsync(p => p.Id == dto.Id.Value);

            if (existingById != null)
            {
                UpdatePersonParty(existingById, dto, normalizedEmail, normalizedPn);
                return existingById;
            }
        }

        // 1) Try match by PersonalNumber
        if (!string.IsNullOrWhiteSpace(normalizedPn))
        {
            var existingByPn = await _uow.OrderParties.Query()
                .OfType<PersonParty>()
                .FirstOrDefaultAsync(p => p.PersonalNumber == normalizedPn);

            if (existingByPn != null)
            {
                UpdatePersonParty(existingByPn, dto, normalizedEmail, normalizedPn);
                return existingByPn;
            }
        }

        // 2) Fallback: match by Email
        if (!string.IsNullOrWhiteSpace(normalizedEmail))
        {
            var existingByEmail = await _uow.OrderParties.Query()
                .OfType<PersonParty>()
                .FirstOrDefaultAsync(p => p.Email == normalizedEmail);

            if (existingByEmail != null)
            {
                UpdatePersonParty(existingByEmail, dto, normalizedEmail, normalizedPn);
                return existingByEmail;
            }
        }

        // 3) Create new
        var (first, last) = SplitFullName(dto.FullName);

        var party = new PersonParty
        {
            FirstName = first,
            LastName = last,
            Email = normalizedEmail ?? string.Empty,
            Phone = dto.Phone,
            PersonalNumber = normalizedPn
        };

        await _uow.OrderParties.AddAsync(party);
        return party;
    }

    private void UpdatePersonParty(PersonParty party, PartyCreateDto dto, string? normalizedEmail, string? normalizedPn)
    {
        var (first, last) = SplitFullName(dto.FullName);

        if (!string.IsNullOrWhiteSpace(first))
            party.FirstName = first;

        if (!string.IsNullOrWhiteSpace(last))
            party.LastName = last;

        if (!string.IsNullOrWhiteSpace(normalizedEmail))
            party.Email = normalizedEmail;

        if (!string.IsNullOrWhiteSpace(dto.Phone))
            party.Phone = dto.Phone;

        if (!string.IsNullOrWhiteSpace(normalizedPn))
            party.PersonalNumber = normalizedPn;
    }

    private static (string firstName, string lastName) SplitFullName(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return (string.Empty, string.Empty);

        var parts = fullName.Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 1)
            return (parts[0], string.Empty);

        return (parts[0], string.Join(" ", parts.Skip(1)));
    }

    private static string? NormalizeEmail(string? email)
        => string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLower();

    private static string? NormalizePersonalNumber(string? pn)
        => string.IsNullOrWhiteSpace(pn) ? null : pn.Trim();



    private static void ApplyFullName(PersonParty party, string fullName)
    {
        fullName = (fullName ?? "").Trim();
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
        {
            party.FirstName = "";
            party.LastName = "";
            return;
        }

        if (parts.Length == 1)
        {
            party.FirstName = parts[0];
            party.LastName = "";
            return;
        }

        party.FirstName = string.Join(" ", parts[..^1]);
        party.LastName = parts[^1];
    }


    private async Task<Tour> CreateTourGraphAsync(OrderCreateDto dto)
    {
        var tour = new Tour
        {
            Name = dto.Destination,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            PassengerCount = dto.PassengerCount
        };

        // Supplier
        var supplier = _mapper.Map<Supplier>(dto.Supplier);
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
        foreach (var t in dto.AirTickets)
        {
            var ticket = _mapper.Map<AirTicket>(t);
            ticket.Tour = tour;

            var price = _mapper.Map<PriceCurrency>(t.Price);
            price.EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow);
            ticket.PriceCurrency = price;

            await _uow.PriceCurrencies.AddAsync(price);
            await _uow.AirTickets.AddAsync(ticket);
        }

        // Hotel bookings
        foreach (var h in dto.HotelBookings)
        {
            var hotel = _mapper.Map<HotelBooking>(h);
            hotel.Tour = tour;

            var price = _mapper.Map<PriceCurrency>(h.Price);
            price.EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow);
            hotel.PriceCurrency = price;

            await _uow.PriceCurrencies.AddAsync(price);
            await _uow.HotelBookings.AddAsync(hotel);
        }

        return tour;
    }


    private async Task<Order?> LoadOrderGraphAsync(Guid orderId)
    {
        return await _uow.Orders.Query()
            .Include(o => o.OrderParty)
            .Include(o => o.CustomerBankRequisites)
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
        // Update current party (ensure it's PersonParty)
        if (order.OrderParty is not PersonParty person)
        {
            person = new PersonParty();
            await _uow.OrderParties.AddAsync(person);
            order.OrderParty = person;
        }

        ApplyFullName(person, dto.FullName);
        person.Email = dto.Email;
        person.Phone = dto.Phone;
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

        foreach (var tDto in dto.AirTickets)
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

        foreach (var hDto in dto.HotelBookings)
        {
            var hotel = _mapper.Map<HotelBooking>(hDto);
            hotel.Tour = order.Tour;

            var price = _mapper.Map<PriceCurrency>(hDto.Price);
            hotel.PriceCurrency = price;

            await _uow.PriceCurrencies.AddAsync(price);
            await _uow.HotelBookings.AddAsync(hotel);
        }
    }

    public async Task<List<SavedCustomerDto>> GetSavedCustomersAsync()
    {
        var customers = await _uow.Orders.Query()
            .GroupBy(o => o.OrderPartyId)
            .Where(g => g.Count() >= 2)
            .Select(g => g.Key)
            .Join(
                _uow.OrderParties.Query().OfType<PersonParty>(),
                partyId => partyId,
                p => p.Id,
                (partyId, p) => new SavedCustomerDto
                {
                    Id = p.Id,
                    FullName = (p.FirstName + " " + p.LastName).Trim()
                }
            )
            .ToListAsync();

        return customers;
    }

public async Task<List<OrderDto>> SearchAsync(string? tourName, DateOnly? startDate, DateOnly? endDate)
{
    var q = _uow.Orders.Query()
        .Include(o => o.OrderParty)
        .Include(o => o.CustomerBankRequisites)
        .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
        .Include(o => o.Tour).ThenInclude(t => t.Passengers)
        .Include(o => o.Tour).ThenInclude(t => t.AirTickets).ThenInclude(a => a.PriceCurrency)
        .Include(o => o.Tour).ThenInclude(t => t.HotelBookings).ThenInclude(h => h.PriceCurrency)
        .Include(o => o.Tour).ThenInclude(t => t.ExtraServices).ThenInclude(e => e.PriceCurrency)
        .Include(o => o.Tour).ThenInclude(t => t.TourSupplier)
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(tourName))
    {
        var term = tourName.Trim().ToLower();
          q = q.Where(o => o.Tour != null && EF.Functions.ILike(o.Tour.Name, $"%{tourName.Trim()}%"));
           
        }

    if (startDate.HasValue)
        q = q.Where(o => o.Tour != null && o.Tour.StartDate >= startDate.Value);

    if (endDate.HasValue)
        q = q.Where(o => o.Tour != null && o.Tour.EndDate <= endDate.Value);

    var orders = await q.ToListAsync();
    return _mapper.Map<List<OrderDto>>(orders);
}

  

    // ---------------------------------------------------
    // REPORT QUERIES (flat DTO)
    // ---------------------------------------------------
    public async Task<OrderReportDto?> GetByIdReportAsync(Guid id)
    {
        var order = await LoadOrdersWithFullGraph()
            .FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return null;
        return MapToReportDtos(new List<Order> { order }).FirstOrDefault();
    }

    public async Task<List<OrderReportDto>> GetAllReportAsync()
    {
        var orders = await LoadOrdersWithFullGraph().ToListAsync();
        return MapToReportDtos(orders);
    }

    public async Task<List<OrderReportDto>> GetReportByStatusAsync(OrderStatus status)
    {
        var orders = await LoadOrdersWithFullGraph()
            .Where(o => o.Status == status)
            .ToListAsync();
        return MapToReportDtos(orders);
    }

    public async Task<List<OrderReportDto>> GetReportByPartyAsync(Guid partyId)
    {
        var orders = await LoadOrdersWithFullGraph()
            .Where(o => o.OrderPartyId == partyId)
            .ToListAsync();
        return MapToReportDtos(orders);
    }

    public async Task<List<OrderReportDto>> GetReportByDateRangeAsync(DateTime start, DateTime end)
    {
        var orders = await LoadOrdersWithFullGraph()
            .Where(o => o.CreatedAtUtc >= start && o.CreatedAtUtc <= end)
            .ToListAsync();
        return MapToReportDtos(orders);
    }

    public async Task<List<OrderReportDto>> SearchReportAsync(string? tourName, DateOnly? startDate, DateOnly? endDate)
    {
        var q = LoadOrdersWithFullGraph();

        if (!string.IsNullOrWhiteSpace(tourName))
            q = q.Where(o => o.Tour != null && EF.Functions.ILike(o.Tour.Name, $"%{tourName.Trim()}%"));

        if (startDate.HasValue)
            q = q.Where(o => o.Tour != null && o.Tour.StartDate >= startDate.Value);

        if (endDate.HasValue)
            q = q.Where(o => o.Tour != null && o.Tour.EndDate <= endDate.Value);

        var orders = await q.ToListAsync();
        return MapToReportDtos(orders);
    }

    private IQueryable<Order> LoadOrdersWithFullGraph()
    {
        return _uow.Orders.Query()
            .Include(o => o.OrderParty)
            .Include(o => o.CustomerBankRequisites)
            .Include(o => o.Payments).ThenInclude(p => p.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.Passengers)
            .Include(o => o.Tour).ThenInclude(t => t.AirTickets).ThenInclude(a => a.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.HotelBookings).ThenInclude(h => h.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.ExtraServices).ThenInclude(e => e.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.TourSupplier);
    }

    private List<OrderReportDto> MapToReportDtos(List<Order> orders)
    {
        var sorted = orders.OrderBy(o => o.Id).ToList();
        var result = new List<OrderReportDto>(sorted.Count);

        foreach (var o in sorted)
        {
            var totalPaid = o.Payments
                .Where(p => p.PriceCurrency != null)
                .Sum(p => p.PriceCurrency!.PriceInGel ?? 0);

            var passengers = o.Tour?.Passengers ?? (ICollection<Passenger>)new List<Passenger>();
            var passengerNames = string.Join(", ",
                passengers.Select(p => $"{p.FirstName} {p.LastName}".Trim()));

            var clientName = string.Empty;
            if (o.OrderParty is PersonParty person)
                clientName = $"{person.FirstName} {person.LastName}".Trim();

            result.Add(new OrderReportDto
            {
                Id = o.Id,
                OrderNo = o.OrderNumber,
                ClientName = clientName,
                NumberOfPax = o.Tour?.PassengerCount ?? 0,
                ListOfPassengers = passengerNames,
                OrderCreationDate = o.CreatedAtUtc,
                ManagerName = o.ManagerName,
                TourName = o.Tour?.Name,
                TourType = o.TourType,
                StartDate = o.Tour?.StartDate ?? default,
                EndDate = o.Tour?.EndDate ?? default,
                GrossPrice = o.SellPriceInGel,
                TicketNet = o.TicketNet,
                TicketSupplier = o.TicketSupplier,
                HotelNet = o.HotelNet,
                HotelSupplier = o.HotelSupplier,
                TransferNet = o.TransferNet,
                TransferSupplier = o.TransferSupplier,
                InsuranceNet = o.InsuranceNet,
                InsuranceSupplier = o.InsuranceSupplier,
                OtherServiceNet = o.OtherServiceNet,
                OtherServiceSupplier = o.OtherServiceSupplier,
                Profit = o.SellPriceInGel - o.TotalExpenseInGel,
                PaidByClient = totalPaid,
                LeftToPay = o.SellPriceInGel - totalPaid,
                Currency = "GEL"
            });
        }

        return result;
    }

    //private async Task RebuildExtraServicesAsync(Order order, OrderEditDto dto)
    //{
    //    foreach (var e in order.Tour!.ExtraServices.ToList())
    //    {
    //        if (e.PriceCurrency != null)
    //            await _uow.PriceCurrencies.RemoveAsync(e.PriceCurrency);

    //        await _uow.ExtraServices.RemoveAsync(e);
    //    }
    //    order.Tour.ExtraServices.Clear();

    //    foreach (var eDto in dto.Tour.ExtraServices)
    //    {
    //        var extra = _mapper.Map<ExtraService>(eDto);
    //        extra.Tour = order.Tour;

    //        var price = _mapper.Map<PriceCurrency>(eDto.Price);
    //        extra.PriceCurrency = price;

    //        await _uow.PriceCurrencies.AddAsync(price);
    //        await _uow.ExtraServices.AddAsync(extra);
    //    }
    //}

    //private async Task RebuildPaymentsAsync(Order order, IEnumerable<PaymentCreateDto> paymentDtos)
    //{
    //    // remove existing
    //    foreach (var existing in order.Payments.ToList())
    //    {
    //        if (existing.PriceCurrency != null)
    //            await _uow.PriceCurrencies.RemoveAsync(existing.PriceCurrency);

    //        await _uow.Payments.RemoveAsync(existing);
    //    }
    //    order.Payments.Clear();

    //    // add new
    //    foreach (var dto in paymentDtos)
    //    {
    //        var price = new PriceCurrency
    //        {
    //            Amount = dto.Price.Amount,
    //            Currency = dto.Price.Currency,
    //            ExchangeRateToGel = dto.Price.ExchangeRateToGel,
    //            EffectiveDate = dto.Price.EffectiveDate
    //        };

    //        await _uow.PriceCurrencies.AddAsync(price);

    //        var payment = new Payment
    //        {
    //            Order = order,
    //            PriceCurrency = price,
    //            BankName = dto.BankName,
    //            PaidDate = dto.PaidDate
    //        };

    //        await _uow.Payments.AddAsync(payment);
    //    }
    //}


}
