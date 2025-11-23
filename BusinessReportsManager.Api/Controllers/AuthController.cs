using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessReportsManager.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    // ===========================
    //        LOGIN
    // ===========================
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request,
        CancellationToken ct)
    {
        try
        {
            var token = await _auth.LoginAsync(request, ct);
            return Ok(token);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    // ===========================
    //    REGISTER EMPLOYEE
    //    NO AUTH REQUIRED
    // ===========================
    [HttpPost("register/employee")]
    [AllowAnonymous] 
    public async Task<ActionResult<RegisterResponse>> RegisterEmployee(
        RegisterRequest request,
        CancellationToken ct)
    {
        try
        {
         
            return Ok(await _auth.RegisterAsync(request, "Employee", ct));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ===========================
    //   REGISTER ACCOUNTANT
    //   SUPERVISOR OR ADMIN
    // ===========================
    [HttpPost("register/accountant")]
    [Authorize(Roles = "Supervisor,Accountant")]
    public async Task<ActionResult<RegisterResponse>> RegisterAccountant(
        RegisterRequest request,
        CancellationToken ct)
    {
        try
        {
            return Ok(await _auth.RegisterAsync(request, "Accountant",ct));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ===========================
    //   REGISTER SUPERVISOR
    //   ADMIN ONLY
    // ===========================
    [HttpPost("register/supervisor")]
    [Authorize(Roles = "Supervisor")]
    public async Task<ActionResult<RegisterResponse>> RegisterSupervisor(
        RegisterRequest request,
        CancellationToken ct)
    {
        try
        {
            return Ok(await _auth.RegisterAsync(request, "Supervisor", ct));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
