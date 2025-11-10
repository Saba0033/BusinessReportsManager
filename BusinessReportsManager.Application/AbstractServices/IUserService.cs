using BusinessReportsManager.Application.DTOs;

namespace BusinessReportsManager.Application.AbstractServices;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(string userId, CancellationToken ct = default);
    Task<string?> GetEmailAsync(string userId, CancellationToken ct = default);
    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct = default);
}