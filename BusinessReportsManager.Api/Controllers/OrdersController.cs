using BusinessReportsManager.Application.Common;
using BusinessReportsManager.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BusinessReportsManager.Application.AbstractServices;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace BusinessReportsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orders;
    private readonly IHttpContextAccessor _http;

    public OrdersController(IOrderService orders, IHttpContextAccessor http)
    {
        _orders = orders;
        _http = http;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool CanViewAll => User.IsInRole("Accountant") || User.IsInRole("Supervisor");
    private bool CanEditAll => User.IsInRole("Accountant") || User.IsInRole("Supervisor");

    [HttpPost]
    // [SwaggerOperation(Summary = "Create order", Description = "Auto-generates OrderNumber.")]
    [SwaggerRequestExample(typeof(CreateOrderDto), typeof(BusinessReportsManager.Api.Extensions.CreateOrderExample))]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        var id = await _orders.CreateAsync(dto, UserId, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet]
    // [SwaggerOperation(Summary = "List orders (paged)", Description = "Employees see their own orders; Accountant/Supervisor can view all.")]
    public async Task<ActionResult<PagedResult<OrderListItemDto>>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _orders.GetPagedAsync(new PagedRequest(page, pageSize), UserId, CanViewAll, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDetailsDto>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var dto = await _orders.GetAsync(id, UserId, CanViewAll, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Employee,Accountant,Supervisor")]
    public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] UpdateOrderDto dto, CancellationToken ct)
    {
        await _orders.UpdateAsync(id, dto, UserId, CanEditAll, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/finalize")]
    [Authorize(Roles = "Accountant,Supervisor")]
    public async Task<ActionResult> Finalize([FromRoute] Guid id, CancellationToken ct)
    {
        await _orders.FinalizeAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/reopen")]
    [Authorize(Roles = "Supervisor")]
    public async Task<ActionResult> Reopen([FromRoute] Guid id, CancellationToken ct)
    {
        await _orders.ReopenAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/passengers")]
    [Authorize(Roles = "Employee,Accountant,Supervisor")]
    [SwaggerRequestExample(typeof(CreatePassengerDto), typeof(BusinessReportsManager.Api.Extensions.CreatePassengerExample))]
    public async Task<ActionResult<PassengerDto>> AddPassenger([FromRoute] Guid id, [FromBody] CreatePassengerDto dto, CancellationToken ct)
    {
        var result = await _orders.AddPassengerAsync(id, dto, UserId, CanEditAll, ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}/passengers/{pid:guid}")]
    [Authorize(Roles = "Employee,Accountant,Supervisor")]
    public async Task<ActionResult> DeletePassenger([FromRoute] Guid id, [FromRoute] Guid pid, CancellationToken ct)
    {
        await _orders.DeletePassengerAsync(id, pid, UserId, CanEditAll, ct);
        return NoContent();
    }
}