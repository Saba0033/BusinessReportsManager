using AutoMapper;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Application.Services;
using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.Interfaces;
using BusinessReportsManager.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BusinessReportsManager.Infrastructure.Services;

public class OrderPartyService : IOrderPartyService
{
    private readonly IGenericRepository _repo;
    private readonly IMapper _mapper;

    public OrderPartyService(IGenericRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Guid> CreateCompanyAsync(CreateCompanyPartyDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<CompanyParty>(dto);
        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<Guid> CreatePersonAsync(CreatePersonPartyDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<PersonParty>(dto);
        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.Query<OrderParty>(asNoTracking: false)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity != null)
        {
            await _repo.Remove(entity);
            await _repo.SaveChangesAsync(ct);
        }
    }

    public async Task<IReadOnlyList<OrderPartyDto>> GetAllAsync(CancellationToken ct = default)
    {
        var persons = await _repo.Query<PersonParty>().ToListAsync(ct);
        var companies = await _repo.Query<CompanyParty>().ToListAsync(ct);

        var result = new List<OrderPartyDto>(persons.Count + companies.Count);
        result.AddRange(persons.Select(p => _mapper.Map<PersonPartyDto>(p)));
        result.AddRange(companies.Select(c => _mapper.Map<CompanyPartyDto>(c)));
        return result;
    }

    public async Task<OrderPartyDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var p = await _repo.GetByIdAsync<PersonParty>(id, ct);
        if (p != null) return _mapper.Map<PersonPartyDto>(p);

        var c = await _repo.GetByIdAsync<CompanyParty>(id, ct);
        if (c != null) return _mapper.Map<CompanyPartyDto>(c);

        return null;
    }
}