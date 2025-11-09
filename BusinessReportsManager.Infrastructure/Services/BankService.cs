using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Application.Services;
using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessReportsManager.Infrastructure.Services;

public class BankService : IBankService
{
    private readonly IGenericRepository _repo;
    private readonly IMapper _mapper;

    public BankService(IGenericRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Guid> CreateAsync(CreateBankDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Bank>(dto);
        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(Guid id, CreateBankDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync<Bank>(id, ct) ?? throw new KeyNotFoundException();
        entity.Name = dto.Name;
        entity.Swift = dto.Swift;
        entity.AccountNumber = dto.AccountNumber;
        await _repo.Update(entity);
        await _repo.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync<Bank>(id, ct);
        if (entity != null)
        {
            await _repo.Remove(entity);
            await _repo.SaveChangesAsync(ct);
        }
    }

    public async Task<IReadOnlyList<BankDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repo.Query<Bank>()
            .OrderBy(x => x.Name)
            .ProjectTo<BankDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
}