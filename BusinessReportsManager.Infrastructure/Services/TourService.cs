using AutoMapper;
using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Application.DTOs.Tour;
using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessReportsManager.Infrastructure.Services;



public class TourService : ITourService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public TourService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<TourDto?> GetTourAsync(Guid id)
    {
        var tour = await _uow.Tours.GetByIdAsync(id);
        return tour == null ? null : _mapper.Map<TourDto>(tour);
    }

    public async Task<List<TourDto>> GetAllToursAsync()
    {
        var tours = await _uow.Tours.Query().ToListAsync();
        return _mapper.Map<List<TourDto>>(tours);
    }

    public async Task<TourDto> CreateTourAsync(string name, DateOnly start, DateOnly end, int passengerCount, Guid supplierId)
    {
        var tour = new Tour
        {
            Name = name,
            StartDate = start,
            EndDate = end,
            PassengerCount = passengerCount,
            TourSupplierId = supplierId
        };

        await _uow.Tours.AddAsync(tour);
        await _uow.SaveChangesAsync();

        return _mapper.Map<TourDto>(tour);
    }

    public async Task<bool> DeleteTourAsync(Guid id)
    {
        var tour = await _uow.Tours.GetByIdAsync(id);
        if (tour == null) return false;

        await _uow.Tours.RemoveAsync(tour);
        await _uow.SaveChangesAsync();

        return true;
    }
}