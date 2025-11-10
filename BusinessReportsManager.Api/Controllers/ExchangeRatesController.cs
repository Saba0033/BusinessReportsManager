using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessReportsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExchangeRatesController : ControllerBase
{
    private readonly IExchangeRateService _service;

    public ExchangeRatesController(IExchangeRateService service) => _service = service;

    /// <summary>
    /// GET /api/exchange-rates
    /// If from/to/date are provided, returns the effective rate on or before the date.
    /// Otherwise returns all rates.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] Currency? from, [FromQuery] Currency? to, [FromQuery] DateOnly? date, CancellationToken ct)
    {
        if (from.HasValue && to.HasValue && date.HasValue)
        {
            var rate = await _service.GetEffectiveAsync(from.Value, to.Value, date.Value, ct);
            return rate is null ? NotFound() : Ok(rate);
        }

        var all = await _service.GetAllAsync(ct);
        return Ok(all);
    }

    [HttpPost]
    [Authorize(Roles = "Accountant,Supervisor")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateExchangeRateDto dto, CancellationToken ct)
    {
        var id = await _service.CreateAsync(dto, ct);
        return Created(nameof(Get), id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Accountant,Supervisor")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateExchangeRateDto dto, CancellationToken ct)
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