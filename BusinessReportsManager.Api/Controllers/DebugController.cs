using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessReportsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    [HttpGet("whoami")]
    [Authorize]
    public IActionResult WhoAmI()
    {
        var name = User.Identity?.Name ?? "Anonymous";
        var roles = User.Claims
            .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value);
        return Ok(new { name, roles });
    }
}