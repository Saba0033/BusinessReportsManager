using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Domain.Enums;
using System.Security.Claims;

namespace BusinessReportsManager.Application.AbstractServices;

public interface IOrderService
{
    // CREATE
    Task<OrderDto> CreateFullOrderAsync(OrderCreateDto dto, ClaimsPrincipal user);

    // EDIT
    Task<OrderDto?> EditOrderAsync(Guid orderId, OrderEditDto dto, ClaimsPrincipal user);

    // PAYMENTS
    Task<PaymentDto?> AddPaymentAsync(Guid orderId, PaymentCreateDto dto, ClaimsPrincipal user);
    Task<bool> RemovePaymentAsync(Guid paymentId, ClaimsPrincipal user);

    // STATUS CHANGE
    Task<bool> ChangeStatusAsync(Guid orderId, OrderStatus newStatus, ClaimsPrincipal user);

    // DELETE
    Task<bool> DeleteOrderAsync(Guid orderId, ClaimsPrincipal user);

    // GET METHODS (all require user)
    Task<List<OrderDto>> GetAllAsync(ClaimsPrincipal user);
    Task<OrderDto?> GetByIdAsync(Guid id, ClaimsPrincipal user);
    Task<List<OrderDto>> GetByStatusAsync(OrderStatus status, ClaimsPrincipal user);
    Task<List<OrderDto>> GetByPartyAsync(Guid orderPartyId, ClaimsPrincipal user);
    Task<List<OrderDto>> GetByDateRangeAsync(DateTime start, DateTime end, ClaimsPrincipal user);

    // SUM
    Task<decimal> GetTotalPaidAsync(Guid orderId);
}
