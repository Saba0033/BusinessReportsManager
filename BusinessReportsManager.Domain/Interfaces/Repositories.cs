using BusinessReportsManager.Domain.Entities;

namespace BusinessReportsManager.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    IQueryable<Order> Query();
    Task SaveChangesAsync(CancellationToken ct = default);
}

public interface ISupplierRepository
{
    IQueryable<Supplier> Query();
    Task<Supplier?> GetAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Supplier entity, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

public interface IBankRepository
{
    IQueryable<Bank> Query();
    Task<Bank?> GetAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Bank entity, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

public interface IExchangeRateRepository
{
    IQueryable<ExchangeRate> Query();
    Task AddAsync(ExchangeRate entity, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

public interface ITourRepository
{
    IQueryable<Tour> Query();
    Task<Tour?> GetAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Tour entity, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

public interface IOrderPartyRepository
{
    IQueryable<OrderParty> Query();
    Task<OrderParty?> GetAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(OrderParty entity, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}