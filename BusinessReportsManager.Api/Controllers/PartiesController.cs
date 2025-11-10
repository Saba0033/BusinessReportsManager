using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessReportsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PartiesController : ControllerBase
{
    private readonly IOrderPartyService _service;

    public PartiesController(IOrderPartyService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderPartyDto>>> GetAll(CancellationToken ct) =>
        Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderPartyDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await _service.GetAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost("person")]
    public async Task<ActionResult<Guid>> CreatePerson([FromBody] CreatePersonPartyDto dto, CancellationToken ct)
    {
        var id = await _service.CreatePersonAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpPost("company")]
    public async Task<ActionResult<Guid>> CreateCompany([FromBody] CreateCompanyPartyDto dto, CancellationToken ct)
    {
        var id = await _service.CreateCompanyAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}