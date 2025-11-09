using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace BusinessReportsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ToursController : ControllerBase
{
    private readonly ITourService _service;

    public ToursController(ITourService service) => _service = service;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TourDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await _service.GetAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TourDto>>> Filter([FromQuery] DateOnly? start, [FromQuery] DateOnly? end, [FromQuery] Guid? supplierId, [FromQuery] string? destination, CancellationToken ct)
    {
        var list = await _service.GetFilteredAsync(start, end, supplierId, destination, ct);
        return Ok(list);
    }

    [HttpPost]
    [Authorize(Roles = "Accountant,Supervisor")]
    [SwaggerRequestExample(typeof(CreateTourDto), typeof(BusinessReportsManager.Api.Extensions.CreateTourExample))]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateTourDto dto, CancellationToken ct)
    {
        var id = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Accountant,Supervisor")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateTourDto dto, CancellationToken ct)
    {
        await _service.UpdateAsync(id, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Supervisor")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}