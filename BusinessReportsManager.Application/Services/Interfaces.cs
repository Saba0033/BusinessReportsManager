using BusinessReportsManager.Application.Common;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Application.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
}

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

public interface ISupplierService
{
    Task<Guid> CreateAsync(CreateSupplierDto dto, CancellationToken ct = default);
    Task UpdateAsync(Guid id, CreateSupplierDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<SupplierDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<SupplierDto>> GetAllAsync(CancellationToken ct = default);
}

public interface IBankService
{
    Task<Guid> CreateAsync(CreateBankDto dto, CancellationToken ct = default);
    Task UpdateAsync(Guid id, CreateBankDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<BankDto>> GetAllAsync(CancellationToken ct = default);
}

public interface IExchangeRateService
{
    Task<Guid> CreateAsync(CreateExchangeRateDto dto, CancellationToken ct = default);
    Task UpdateAsync(Guid id, CreateExchangeRateDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ExchangeRateDto>> GetAllAsync(CancellationToken ct = default);
    Task<ExchangeRateDto?> GetEffectiveAsync(Currency from, Currency to, DateOnly date, CancellationToken ct = default);
    decimal Convert(Currency from, Currency to, decimal amount, DateOnly date);
}

public interface ITourService
{
    Task<Guid> CreateAsync(CreateTourDto dto, CancellationToken ct = default);
    Task<TourDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TourDto>> GetFilteredAsync(DateOnly? start, DateOnly? end, Guid? supplierId, string? destination, CancellationToken ct = default);
    Task UpdateAsync(Guid id, CreateTourDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IOrderPartyService
{
    Task<Guid> CreatePersonAsync(CreatePersonPartyDto dto, CancellationToken ct = default);
    Task<Guid> CreateCompanyAsync(CreateCompanyPartyDto dto, CancellationToken ct = default);
    Task<OrderPartyDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<OrderPartyDto>> GetAllAsync(CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}