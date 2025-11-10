using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BusinessReportsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    /// <summary>Login and get a JWT.</summary>
    [HttpPost("login")]
    // [SwaggerOperation(Summary = "Login (JWT)", Description = "Returns a JWT token with role claims.")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var token = await _auth.LoginAsync(request, ct);
        return Ok(token);
    }
}