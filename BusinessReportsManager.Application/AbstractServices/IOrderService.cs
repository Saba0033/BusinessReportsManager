using BusinessReportsManager.Application.DTOs.Order;
using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Application.AbstractServices;

public interface IOrderService
{
    Task<OrderDto> CreateFullOrderAsync(OrderCreateDto dto);
    Task<OrderDto?> EditOrderAsync(Guid orderId, OrderEditDto dto);

    Task<bool> ChangeStatusAsync(Guid orderId, OrderStatus newStatus);
    Task<bool> DeleteOrderAsync(Guid orderId);

    Task<List<OrderDto>> GetAllAsync();
    Task<OrderDto?> GetByIdAsync(Guid id);
    Task<List<OrderDto>> GetByStatusAsync(OrderStatus status);
    Task<List<OrderDto>> GetByPartyAsync(Guid partyId);
    Task<List<OrderDto>> GetByDateRangeAsync(DateTime start, DateTime end);
}