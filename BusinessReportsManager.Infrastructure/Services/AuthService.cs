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
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenGenerator _jwt;

    public AuthService(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IJwtTokenGenerator jwt,
        RoleManager<IdentityRole> roleManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwt = jwt;
        _roleManager = roleManager;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            throw new UnauthorizedAccessException("Invalid credentials");

        var check = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: false
        );

        if (!check.Succeeded)
            throw new UnauthorizedAccessException("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expires) = _jwt.GenerateToken(user, roles);

        return new LoginResponse(token, expires);
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, string role, CancellationToken ct = default)
    {
        // Check if user exists
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing != null)
            throw new Exception("User already exists.");

        // Create AppUser
        var user = new AppUser
        {
            Email = request.Email,
            UserName = request.Email,
            FullName = request.FullName
        };

        var created = await _userManager.CreateAsync(user, request.Password);
        if (!created.Succeeded)
        {
            string errors = string.Join(", ", created.Errors.Select(e => e.Description));
            throw new Exception(errors);
        }

        // Assign role (use Employee if none is provided)
        string targetRole = string.IsNullOrWhiteSpace(role)
            ? "Employee"
            : role!;

        if (!await _roleManager.RoleExistsAsync(targetRole))
            throw new Exception($"Role '{targetRole}' does not exist.");

        await _userManager.AddToRoleAsync(user, targetRole);

        return new RegisterResponse(user.Id, user.Email!, user.FullName!);
    }
}
