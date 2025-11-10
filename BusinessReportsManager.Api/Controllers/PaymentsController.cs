using BusinessReportsManager.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BusinessReportsManager.Application.AbstractServices;
using Swashbuckle.AspNetCore.Filters;

namespace BusinessReportsManager.Api.Controllers;

[ApiController]
[Route("api/orders/{orderId:guid}/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IOrderService _orders;

    public PaymentsController(IOrderService orders) => _orders = orders;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool CanViewAll => User.IsInRole("Accountant") || User.IsInRole("Supervisor");
    private bool CanEditAll => User.IsInRole("Accountant") || User.IsInRole("Supervisor");

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PaymentDto>>> GetAll([FromRoute] Guid orderId, CancellationToken ct)
    {
        var list = await _orders.GetPaymentsAsync(orderId, UserId, CanViewAll, ct);
        return Ok(list);
    }

    [HttpPost]
    [Authorize(Roles = "Employee,Accountant,Supervisor")]
    [SwaggerRequestExample(typeof(CreatePaymentDto), typeof(BusinessReportsManager.Api.Extensions.CreatePaymentExample))]
    public async Task<ActionResult<PaymentDto>> Create([FromRoute] Guid orderId, [FromBody] CreatePaymentDto dto, CancellationToken ct)
    {
        var item = await _orders.AddPaymentAsync(orderId, dto, UserId, CanEditAll, ct);
        return Ok(item);
    }

    [HttpDelete("{paymentId:guid}")]
    [Authorize(Roles = "Employee,Accountant,Supervisor")]
    public async Task<ActionResult> Delete([FromRoute] Guid orderId, [FromRoute] Guid paymentId, CancellationToken ct)
    {
        await _orders.DeletePaymentAsync(orderId, paymentId, UserId, CanEditAll, ct);
        return NoContent();
    }
}