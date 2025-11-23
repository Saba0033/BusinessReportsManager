using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BusinessReportsManager.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;

    public UserService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserDto?> GetByIdAsync(string userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user is null
            ? null
            : new UserDto(user.Id, user.Email ?? string.Empty, user.FullName);
    }

    public async Task<string?> GetEmailAsync(string userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.Email;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct = default)
    {
        // IQueryable is supported when using EF-backed stores
        var users = await _userManager.Users
            .Select(u => new UserDto(u.Id, u.Email ?? string.Empty, u.FullName))
            .ToListAsync(ct);

        return users;
    }
}