using AutoMapper;
using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs.Payment;
using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessReportsManager.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public PaymentService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    // -----------------------------------------
    // ADD PAYMENT (Customer → Company)
    // -----------------------------------------
    public async Task<PaymentDto?> AddPaymentAsync(Guid orderId, PaymentCreateDto dto)
    {
        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order == null) return null;

        var price = new PriceCurrency
        {
            Amount = dto.Price.Amount,
            Currency = dto.Price.Currency,
            ExchangeRateToGel = dto.Price.ExchangeRateToGel,
            EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        await _uow.PriceCurrencies.AddAsync(price);

        var payment = new Payment
        {
            OrderId = orderId,
            PriceCurrency = price,
            BankName = dto.BankName,
            PaidDate = dto.PaidDate
        };

        await _uow.Payments.AddAsync(payment);
        await _uow.SaveChangesAsync();

        return _mapper.Map<PaymentDto>(payment);
    }

    // -----------------------------------------
    // REMOVE PAYMENT
    // -----------------------------------------
    public async Task<bool> RemovePaymentAsync(Guid paymentId)
    {
        var payment = await _uow.Payments.Query()
            .Include(p => p.PriceCurrency)
            .FirstOrDefaultAsync(p => p.Id == paymentId);

        if (payment == null) return false;

        if (payment.PriceCurrency != null)
            await _uow.PriceCurrencies.RemoveAsync(payment.PriceCurrency);

        await _uow.Payments.RemoveAsync(payment);
        await _uow.SaveChangesAsync();
        return true;
    }

    // -----------------------------------------
    // CUSTOMER PAID TOTAL
    // -----------------------------------------
    public async Task<decimal> GetTotalPaidAsync(Guid orderId)
    {
        return await _uow.Payments.Query(p => p.OrderId == orderId)
            .Include(p => p.PriceCurrency)
            .SumAsync(p => p.PriceCurrency.Amount * (p.PriceCurrency.ExchangeRateToGel ?? 1));
    }

    // -----------------------------------------
    // EXPENSES (AirTickets + Hotel + ExtraService)
    // -----------------------------------------
    public async Task<decimal> GetExpensesAsync(Guid orderId)
    {
        var order = await _uow.Orders.Query()
            .Include(o => o.Tour).ThenInclude(t => t.AirTickets).ThenInclude(a => a.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.HotelBookings).ThenInclude(h => h.PriceCurrency)
            .Include(o => o.Tour).ThenInclude(t => t.ExtraServices).ThenInclude(e => e.PriceCurrency)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) return 0;

        decimal air = order.Tour.AirTickets.Sum(a =>
            a.PriceCurrency.Amount * (a.PriceCurrency.ExchangeRateToGel ?? 1));

        decimal hotel = order.Tour.HotelBookings.Sum(h =>
            h.PriceCurrency.Amount * (h.PriceCurrency.ExchangeRateToGel ?? 1));

        decimal extra = order.Tour.ExtraServices.Sum(e =>
            e.PriceCurrency.Amount * (e.PriceCurrency.ExchangeRateToGel ?? 1));

        return air + hotel + extra;
    }

    // -----------------------------------------
    // PROFIT
    // -----------------------------------------
    public async Task<decimal> GetProfitAsync(Guid orderId)
    {
        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order == null) return 0;

        decimal expenses = await GetExpensesAsync(orderId);

        return order.SellPriceInGel - expenses;
    }

    // -----------------------------------------
    // SUPPLIER OWED = expenses
    // -----------------------------------------
    public async Task<decimal> GetSupplierOwedAsync(Guid orderId)
    {
        return await GetExpensesAsync(orderId);
    }
}
