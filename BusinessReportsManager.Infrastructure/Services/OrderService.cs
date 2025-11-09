using AutoMapper;
using BusinessReportsManager.Application.Common;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Application.Services;
using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.Enums;
using BusinessReportsManager.Domain.Interfaces;
using BusinessReportsManager.Domain.Queries;
using BusinessReportsManager.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BusinessReportsManager.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IGenericRepository _repo;
    private readonly IMapper _mapper;
    private readonly IOrderNumberGenerator _numberGenerator;
    private readonly IExchangeRateService _rates;
    private readonly IUserService _users;

    public OrderService(
        IGenericRepository repo,
        IMapper mapper,
        IOrderNumberGenerator numberGenerator,
        IExchangeRateService rates,
        IUserService userService)
    {
        _repo = repo;
        _mapper = mapper;
        _numberGenerator = numberGenerator;
        _rates = rates;
        _users = userService;
    }

    public async Task<Guid> CreateAsync(CreateOrderDto dto, string userId, CancellationToken ct = default)
    {
        var party = await _repo.GetByIdAsync<OrderParty>(dto.OrderPartyId, ct) ?? throw new KeyNotFoundException("Order party not found");
        var tour   = await _repo.GetByIdAsync<Tour>(dto.TourId, ct)       ?? throw new KeyNotFoundException("Tour not found");

        var order = new Order
        {
            OrderNumber = await _numberGenerator.NextOrderNumberAsync(ct),
            OrderPartyId = party.Id,
            TourId = tour.Id,
            Source = dto.Source,
            SellPrice = new Money(dto.SellPrice.Amount, dto.SellPrice.Currency),
            TicketSelfCost = new Money(dto.TicketSelfCost.Amount, dto.TicketSelfCost.Currency),
            OwnedByUserId = userId,
            Status = OrderStatus.Open
        };
        await _repo.AddAsync(order, ct);
        await _repo.SaveChangesAsync(ct);
        return order.Id;
    }

    public async Task<OrderDetailsDto?> GetAsync(Guid id, string requesterUserId, bool canViewAll, CancellationToken ct = default)
    {
        var order = await _repo.Query<Order>()
            .Include(o => o.OrderParty)
            .Include(o => o.Tour)
            .Include(o => o.Passengers)
            .Include(o => o.Payments).ThenInclude(p => p.Bank)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        if (order is null) return null;

        if (!canViewAll && order.OwnedByUserId != requesterUserId && order.Status != OrderStatus.Open)
            throw new UnauthorizedAccessException("You can only view your own open orders.");

        var dto = _mapper.Map<OrderDetailsDto>(order);
        dto = dto with
        {
            OwnedByUserEmail = await _users.GetEmailAsync(order.OwnedByUserId, ct) ?? "",
            Payments = order.Payments.Select(p =>
                new PaymentDto(p.Id, new MoneyDto(p.Amount.Amount, p.Amount.Currency), p.BankId, p.Bank?.Name ?? "", p.PaidDate, p.Reference)).ToList(),
            PaymentStatus = GetPaymentStatus(order)
        };
        return dto;
    }

    public async Task<PagedResult<OrderListItemDto>> GetPagedAsync(PagedRequest request, string requesterUserId, bool canViewAll, CancellationToken ct = default)
    {
        var q = _repo.Query<Order>()
            .Include(o => o.OrderParty)
            .Include(o => o.Tour)
            .Include(o => o.Payments);

       

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(o => o.CreatedAtUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var list = new List<OrderListItemDto>(items.Count);
        foreach (var o in items)
        {
            var dto = _mapper.Map<OrderListItemDto>(o);
            dto = dto with
            {
                OwnedByUserEmail = await _users.GetEmailAsync(o.OwnedByUserId, ct) ?? "",
                PaymentStatus = GetPaymentStatus(o)
            };
            list.Add(dto);
        }

        return new PagedResult<OrderListItemDto>
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = total,
            Items = list
        };
    }

    public async Task UpdateAsync(Guid id, UpdateOrderDto dto, string requesterUserId, bool canEditAll, CancellationToken ct = default)
    {
        var order = await _repo.GetByIdAsync<Order>(id, ct) ?? throw new KeyNotFoundException();
        if (!canEditAll && (order.OwnedByUserId != requesterUserId || order.Status != OrderStatus.Open))
            throw new UnauthorizedAccessException("Employees can edit only their own open orders.");

        if (!string.IsNullOrWhiteSpace(dto.RowVersionBase64))
        {
            var clientRowVersion = Convert.FromBase64String(dto.RowVersionBase64);
            await _repo.SetOriginalRowVersion(order, clientRowVersion);
        }

        order.Source = dto.Source ?? order.Source;
        order.SellPrice = new Money(dto.SellPrice.Amount, dto.SellPrice.Currency);
        order.TicketSelfCost = new Money(dto.TicketSelfCost.Amount, dto.TicketSelfCost.Currency);
        order.UpdatedAtUtc = DateTime.UtcNow;

        await _repo.Update(order);
        await _repo.SaveChangesAsync(ct);
    }

    public async Task FinalizeAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _repo.GetByIdAsync<Order>(id, ct) ?? throw new KeyNotFoundException();
        order.Status = OrderStatus.Finalized;
        await _repo.Update(order);
        await _repo.SaveChangesAsync(ct);
    }

    public async Task ReopenAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _repo.GetByIdAsync<Order>(id, ct) ?? throw new KeyNotFoundException();
        order.Status = OrderStatus.Open;
        await _repo.Update(order);
        await _repo.SaveChangesAsync(ct);
    }

    public async Task<PassengerDto> AddPassengerAsync(Guid orderId, CreatePassengerDto dto, string requesterUserId, bool canEditAll, CancellationToken ct = default)
    {
        var orderSlim = await _repo.Query<Order>()
            .Where(o => o.Id == orderId)
            .Select(o => new { o.Id, o.OwnedByUserId, o.Status })
            .FirstOrDefaultAsync(ct) ?? throw new KeyNotFoundException();

        if (!canEditAll && (orderSlim.OwnedByUserId != requesterUserId || orderSlim.Status != OrderStatus.Open))
            throw new UnauthorizedAccessException("Employees can edit only their own open orders.");

        var p = _mapper.Map<Passenger>(dto);
        p.OrderId = orderId;
        await _repo.AddAsync(p, ct);
        await _repo.SaveChangesAsync(ct);
        return _mapper.Map<PassengerDto>(p);
    }

    public async Task DeletePassengerAsync(Guid orderId, Guid passengerId, string requesterUserId, bool canEditAll, CancellationToken ct = default)
    {
        var orderSlim = await _repo.Query<Order>()
            .Where(o => o.Id == orderId)
            .Select(o => new { o.Id, o.OwnedByUserId, o.Status })
            .FirstOrDefaultAsync(ct) ?? throw new KeyNotFoundException();

        if (!canEditAll && (orderSlim.OwnedByUserId != requesterUserId || orderSlim.Status != OrderStatus.Open))
            throw new UnauthorizedAccessException("Employees can edit only their own open orders.");

        var p = await _repo.Query<Passenger>(asNoTracking: false)
            .FirstOrDefaultAsync(x => x.Id == passengerId && x.OrderId == orderId, ct)
            ?? throw new KeyNotFoundException();

        await _repo.Remove(p);
        await _repo.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<PaymentDto>> GetPaymentsAsync(Guid orderId, string requesterUserId, bool canViewAll, CancellationToken ct = default)
    {
        var orderSlim = await _repo.Query<Order>()
            .Where(o => o.Id == orderId)
            .Select(o => new { o.Id, o.OwnedByUserId, o.Status })
            .FirstOrDefaultAsync(ct) ?? throw new KeyNotFoundException();

        if (!canViewAll && orderSlim.OwnedByUserId != requesterUserId && orderSlim.Status != OrderStatus.Open)
            throw new UnauthorizedAccessException("You can only view your own open orders.");

        var payments = await _repo.Query<Payment>()
            .Include(p => p.Bank)
            .Where(p => p.OrderId == orderId)
            .ToListAsync(ct);

        return payments.Select(p =>
            new PaymentDto(p.Id, new MoneyDto(p.Amount.Amount, p.Amount.Currency), p.BankId, p.Bank?.Name ?? "", p.PaidDate, p.Reference)).ToList();
    }

    public async Task<PaymentDto> AddPaymentAsync(Guid orderId, CreatePaymentDto dto, string requesterUserId, bool canEditAll, CancellationToken ct = default)
    {
        var orderSlim = await _repo.Query<Order>()
            .Where(o => o.Id == orderId)
            .Select(o => new { o.Id, o.OwnedByUserId, o.Status })
            .FirstOrDefaultAsync(ct) ?? throw new KeyNotFoundException();

        if (!canEditAll && (orderSlim.OwnedByUserId != requesterUserId || orderSlim.Status != OrderStatus.Open))
            throw new UnauthorizedAccessException("Employees can edit only their own open orders.");

        var bank = await _repo.GetByIdAsync<Bank>(dto.BankId, ct) ?? throw new KeyNotFoundException("Bank not found");

        var payment = new Payment
        {
            OrderId = orderId,
            Amount = new Money(dto.Amount.Amount, dto.Amount.Currency),
            BankId = bank.Id,
            PaidDate = dto.PaidDate,
            Reference = dto.Reference
        };
        await _repo.AddAsync(payment, ct);
        await _repo.SaveChangesAsync(ct);

        return new PaymentDto(
            payment.Id,
            new MoneyDto(payment.Amount.Amount, payment.Amount.Currency),
            bank.Id,
            bank.Name,
            payment.PaidDate,
            payment.Reference);
    }

    public async Task DeletePaymentAsync(Guid orderId, Guid paymentId, string requesterUserId, bool canEditAll, CancellationToken ct = default)
    {
        var orderSlim = await _repo.Query<Order>()
            .Where(o => o.Id == orderId)
            .Select(o => new { o.Id, o.OwnedByUserId, o.Status })
            .FirstOrDefaultAsync(ct) ?? throw new KeyNotFoundException();

        if (!canEditAll && (orderSlim.OwnedByUserId != requesterUserId || orderSlim.Status != OrderStatus.Open))
            throw new UnauthorizedAccessException("Employees can edit only their own open orders.");

        var p = await _repo.Query<Payment>(asNoTracking: false)
            .FirstOrDefaultAsync(x => x.Id == paymentId && x.OrderId == orderId, ct)
            ?? throw new KeyNotFoundException();

        await _repo.Remove(p);
        await _repo.SaveChangesAsync(ct);
    }

    private string GetPaymentStatus(Order order)
    {
        decimal total = 0m;
        foreach (var pay in order.Payments)
        {
            total += _rates.Convert(pay.Amount.Currency, order.SellPrice.Currency, pay.Amount.Amount, pay.PaidDate);
        }

        if (total <= 0) return PaymentStatus.NotPaid.ToString();
        if (total < order.SellPrice.Amount) return PaymentStatus.PartiallyPaid.ToString();
        return PaymentStatus.PaidInFull.ToString();
    }
}