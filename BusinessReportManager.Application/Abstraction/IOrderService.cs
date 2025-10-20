using BusinessReportsManager.Domain.Entities;

namespace BusinessReportsManager.Application.Abstractions;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(CreateOrderDto dto, CancellationToken ct = default);
    Task<Order?> GetOrderAsync(int id, CancellationToken ct = default);
    Task<List<Order>> GetOrdersAsync(bool includeAllForPrivileged, string? createdByUserId, CancellationToken ct = default);
    Task<Order?> UpdateOrderAsync(int id, UpdateOrderDto dto, CancellationToken ct = default);
    Task<bool> CloseOrderAsync(int id, CancellationToken ct = default);
    Task<Payment> AddPaymentAsync(int orderId, CreatePaymentDto dto, CancellationToken ct = default);
}