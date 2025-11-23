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
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthService(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IJwtTokenGenerator jwt, RoleManager<IdentityRole> roleManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwt = jwt;
        _roleManager = roleManager;
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

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        // Email lookup supports ct because it hits database
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing != null)
            throw new Exception("User already exists.");

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName
        };

        // This does NOT accept CT (Identity API limitation)
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception(errors);
        }

        // Optional role assignment
        if (await _roleManager.RoleExistsAsync("Employee"))
            await _userManager.AddToRoleAsync(user, "Employee");

        return new RegisterResponse(user.Id, user.Email!, user.FullName!);
    }

}