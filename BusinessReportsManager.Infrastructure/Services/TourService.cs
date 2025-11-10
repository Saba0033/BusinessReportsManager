using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.Interfaces;
using BusinessReportsManager.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BusinessReportsManager.Infrastructure.Services;

public class TourService : ITourService
{
    private readonly IGenericRepository _repo;
    private readonly IMapper _mapper;

    public TourService(IGenericRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Guid> CreateAsync(CreateTourDto dto, CancellationToken ct = default)
    {
        var entity = new Tour
        {
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            PassengerCount = dto.PassengerCount
        };
        foreach (var d in dto.Destinations)
        {
            entity.Destinations.Add(new Destination { Country = d.Country, City = d.City });
        }

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<TourDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        return await _repo.Query<Tour>()
            .Include(t => t.Destinations)
            .Include(t => t.AirTickets)
            .Include(t => t.HotelBookings)
            .Include(t => t.ExtraServices)
            .Where(t => t.Id == id)
            .ProjectTo<TourDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<TourDto>> GetFilteredAsync(DateOnly? start, DateOnly? end, Guid? supplierId, string? destination, CancellationToken ct = default)
    {
        var q = _repo.Query<Tour>();

        if (start.HasValue) q = q.Where(t => t.StartDate >= start.Value);
        if (end.HasValue) q = q.Where(t => t.EndDate <= end.Value);
        if (!string.IsNullOrWhiteSpace(destination))
            q = q.Where(t => t.Destinations.Any(d => d.Country.Contains(destination) || d.City.Contains(destination)));
        if (supplierId.HasValue)
            q = q.Where(t => t.TourSupplierId == supplierId.Value);

        return await q
            .Include(t => t.Destinations)
            .ProjectTo<TourDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(Guid id, CreateTourDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.Query<Tour>(asNoTracking: false)
            .Include(t => t.Destinations)
            .FirstOrDefaultAsync(t => t.Id == id, ct) ?? throw new KeyNotFoundException();

        entity.Name = dto.Name;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.PassengerCount = dto.PassengerCount;

        // Replace destinations atomically
        if (entity.Destinations.Any())
        {
            await _repo.RemoveRange(entity.Destinations);
            entity.Destinations.Clear();
        }
        foreach (var d in dto.Destinations)
            entity.Destinations.Add(new Destination { Country = d.Country, City = d.City });

        await _repo.Update(entity);
        await _repo.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync<Tour>(id, ct);
        if (entity != null)
        {
            await _repo.Remove(entity);
            await _repo.SaveChangesAsync(ct);
        }
    }
}
