using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Infrastructure.Identity;
using BusinessReportsManager.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;

namespace BusinessReportsManager.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenGenerator _jwt;

    public AuthService(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IJwtTokenGenerator jwt)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwt = jwt;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null) throw new UnauthorizedAccessException("Invalid credentials");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded) throw new UnauthorizedAccessException("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expires) = _jwt.GenerateToken(user, roles);
        return new LoginResponse(token, expires);
    }
}