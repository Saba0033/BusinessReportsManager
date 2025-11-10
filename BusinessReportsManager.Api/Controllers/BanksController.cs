using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessReportsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BanksController : ControllerBase
{
    private readonly IBankService _service;

    public BanksController(IBankService service) => _service = service;

    [HttpGet("AllBanks")]
    public async Task<ActionResult<IReadOnlyList<BankDto>>> GetAll(CancellationToken ct) =>
        Ok(await _service.GetAllAsync(ct));

    [HttpPost("CreateBank")]
    [Authorize(Roles = "Accountant,Supervisor")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateBankDto dto, CancellationToken ct)
    {
        Console.WriteLine("sdcsdfsfasfasfasfafasf");
        var id = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetAll), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Accountant,Supervisor")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateBankDto dto, CancellationToken ct)
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