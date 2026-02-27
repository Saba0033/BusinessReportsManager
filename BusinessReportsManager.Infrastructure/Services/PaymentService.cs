using AutoMapper;
using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs;
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
        var order = await _uow.Orders.GetByIdAsync(orderId);
        return order?.TotalExpenseInGel ?? 0;

    }

    // -----------------------------------------
    // PROFIT
    // -----------------------------------------
    public async Task<decimal> GetProfitAsync(Guid orderId)
    {
        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order == null) return 0;

        return order.SellPriceInGel - order.TotalExpenseInGel;
    }

    // -----------------------------------------
    // SUPPLIER OWED = expenses
    // -----------------------------------------
    public async Task<decimal> GetSupplierOwedAsync(Guid orderId)
    {
        var order = await _uow.Orders.GetByIdAsync(orderId);
        return order?.TotalExpenseInGel ?? 0;
    }
    public async Task<decimal> GetCustomerRemainingAsync(Guid orderId)
    {
        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order == null) return 0;

        var totalPaid = await GetTotalPaidAsync(orderId);

        return order.SellPriceInGel - totalPaid;
    }
    public async Task<decimal> GetCashFlowAsync(Guid orderId)
    {
        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order == null) return 0;

        var totalPaid = await GetTotalPaidAsync(orderId);

        return totalPaid - order.TotalExpenseInGel;
    }
    public async Task<OrderFinancialSummaryDto?> GetFinancialSummaryAsync(Guid orderId)
    {
        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order == null) return null;

        var totalPaid = await GetTotalPaidAsync(orderId);

        return new OrderFinancialSummaryDto
        {
            OrderId = orderId,
            SellPriceInGel = order.SellPriceInGel,
            TotalExpenseInGel = order.TotalExpenseInGel,
            TotalPaidInGel = totalPaid,
            CustomerRemainingInGel = order.SellPriceInGel - totalPaid,
            ProfitInGel = order.SellPriceInGel - order.TotalExpenseInGel,
            CashFlowInGel = totalPaid - order.TotalExpenseInGel
        };
    }


}
