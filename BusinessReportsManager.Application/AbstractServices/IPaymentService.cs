using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Application.DTOs.Payment;

namespace BusinessReportsManager.Application.AbstractServices;

public interface IPaymentService
{
    Task<PaymentDto?> AddPaymentAsync(Guid orderId, PaymentCreateDto dto);
    Task<bool> RemovePaymentAsync(Guid paymentId);

    Task<decimal> GetTotalPaidAsync(Guid orderId);

    Task<decimal> GetExpensesAsync(Guid orderId);
    Task<decimal> GetProfitAsync(Guid orderId);
    Task<decimal> GetSupplierOwedAsync(Guid orderId);

    Task<OrderFinancialSummaryDto?> GetFinancialSummaryAsync(Guid orderId);
}