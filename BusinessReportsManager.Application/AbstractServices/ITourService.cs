using BusinessReportsManager.Application.DTOs;

namespace BusinessReportsManager.Application.AbstractServices;

public interface ITourService
{
    Task<TourDto?> GetTourAsync(Guid id);
    Task<List<TourDto>> GetAllToursAsync();
    Task<TourDto> CreateTourAsync(string name, DateOnly start, DateOnly end, int passengerCount, Guid supplierId);
    Task<bool> DeleteTourAsync(Guid id);
}