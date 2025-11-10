using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.Interfaces;
using BusinessReportsManager.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BusinessReportsManager.Infrastructure.Services;


public class SupplierService : ISupplierService
{
    private readonly IGenericRepository _repo;
    private readonly IMapper _mapper;

    public SupplierService(IGenericRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Guid> CreateAsync(CreateSupplierDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Supplier>(dto);
        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(Guid id, CreateSupplierDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync<Supplier>(id, ct) ?? throw new KeyNotFoundException();
        entity.Name = dto.Name;
        entity.TaxId = dto.TaxId;
        entity.ContactEmail = dto.ContactEmail;
        entity.Phone = dto.Phone;
        await _repo.Update(entity);
        await _repo.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync<Supplier>(id, ct);
        if (entity != null)
        {
            await _repo.Remove(entity);
            await _repo.SaveChangesAsync(ct);
        }
    }

    public async Task<SupplierDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        return await _repo.Query<Supplier>()
            .Where(x => x.Id == id)
            .ProjectTo<SupplierDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<SupplierDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repo.Query<Supplier>()
            .OrderBy(x => x.Name)
            .ProjectTo<SupplierDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
}