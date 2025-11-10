using BusinessReportsManager.Application.Common;
using BusinessReportsManager.Application.DTOs;

namespace BusinessReportsManager.Application.AbstractServices;

public interface IOrderService
{
    Task<Guid> CreateAsync(CreateOrderDto dto, string userId, CancellationToken ct = default);
    Task<OrderDetailsDto?> GetAsync(Guid id, string requesterUserId, bool canViewAll, CancellationToken ct = default);
    Task<PagedResult<OrderListItemDto>> GetPagedAsync(PagedRequest request, string requesterUserId, bool canViewAll, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateOrderDto dto, string requesterUserId, bool canEditAll, CancellationToken ct = default);
    Task FinalizeAsync(Guid id, CancellationToken ct = default);
    Task ReopenAsync(Guid id, CancellationToken ct = default);
    Task<PassengerDto> AddPassengerAsync(Guid orderId, CreatePassengerDto dto, string requesterUserId, bool canEditAll, CancellationToken ct = default);
    Task DeletePassengerAsync(Guid orderId, Guid passengerId, string requesterUserId, bool canEditAll, CancellationToken ct = default);
    Task<IReadOnlyList<PaymentDto>> GetPaymentsAsync(Guid orderId, string requesterUserId, bool canViewAll, CancellationToken ct = default);
    Task<PaymentDto> AddPaymentAsync(Guid orderId, CreatePaymentDto dto, string requesterUserId, bool canEditAll, CancellationToken ct = default);
    Task DeletePaymentAsync(Guid orderId, Guid paymentId, string requesterUserId, bool canEditAll, CancellationToken ct = default);
}