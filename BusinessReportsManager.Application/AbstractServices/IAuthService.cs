using BusinessReportsManager.Application.DTOs;

namespace BusinessReportsManager.Application.AbstractServices;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);

    Task<RegisterResponse> RegisterAsync(RegisterRequest request, string role, CancellationToken ct = default);
}