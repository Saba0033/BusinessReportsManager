using BusinessReportsManager.Application.DTOs;
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
    Task<List<SavedCustomerDto>> GetSavedCustomersAsync();
    Task<List<OrderDto>> SearchAsync(string? tourName, DateOnly? startDate, DateOnly? endDate);
    Task<bool> UpdateAccountingCommentAsync(Guid orderId, string? comment);

    Task<OrderReportDto?> GetByIdReportAsync(Guid id);
    Task<List<OrderReportDto>> GetAllReportAsync();
    Task<List<OrderReportDto>> GetReportByStatusAsync(OrderStatus status);
    Task<List<OrderReportDto>> GetReportByPartyAsync(Guid partyId);
    Task<List<OrderReportDto>> GetReportByDateRangeAsync(DateTime start, DateTime end);
    Task<List<OrderReportDto>> SearchReportAsync(string? tourName, DateOnly? startDate, DateOnly? endDate);
}