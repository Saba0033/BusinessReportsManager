using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs.Order;
using BusinessReportsManager.Application.DTOs.Payment;
using BusinessReportsManager.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace BusinessReportsManager.API.Controllers;

[Authorize]
[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orders;
    private readonly IPaymentService _payments;
    private readonly IOrderExcelExportService _orderExcelExport;

    public OrderController(
        IOrderService orders,
        IPaymentService payments,
        IOrderExcelExportService orderExcelExport)
    {
        _orders = orders;
        _payments = payments;
        _orderExcelExport = orderExcelExport;
    }

    /// <summary>
    /// Creates a new order with party, tour, passengers, services, and payments.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Supervisor,Employee")]
    [ProducesResponseType(typeof(OrderDto), 201)]
    public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
    {
        var result = await _orders.CreateFullOrderAsync(dto);
        return CreatedAtAction(nameof(GetById), new { orderId = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing order by fully replacing its nested structure,
    /// including passengers, tickets, hotel bookings, extra services, and payments.
    /// </summary>
    [HttpPut("{orderId:guid}")]
    [ProducesResponseType(typeof(OrderDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Edit(Guid orderId, [FromBody] OrderEditDto dto)
    {
        var result = await _orders.EditOrderAsync(orderId, dto);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Retrieves all orders with full nested details.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _orders.GetAllAsync());
    }

    /// <summary>
    /// Retrieves a single order by ID.
    /// </summary>
    [HttpGet("{orderId:guid}")]
    [ProducesResponseType(typeof(OrderDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid orderId)
    {
        var result = await _orders.GetByIdAsync(orderId);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Retrieves all orders with a specific status.
    /// </summary>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(List<OrderDto>), 200)]
    public async Task<IActionResult> GetByStatus(OrderStatus status)
    {
        return Ok(await _orders.GetByStatusAsync(status));
    }

    /// <summary>
    /// Retrieves all orders associated with a specific party.
    /// </summary>
    [HttpGet("party/{partyId:guid}")]
    [ProducesResponseType(typeof(List<OrderDto>), 200)]
    public async Task<IActionResult> GetByParty(Guid partyId)
    {
        return Ok(await _orders.GetByPartyAsync(partyId));
    }

    /// <summary>
    /// Retrieves all orders created within a date range.
    /// </summary>
    [HttpGet("date-range")]
    [ProducesResponseType(typeof(List<OrderDto>), 200)]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        return Ok(await _orders.GetByDateRangeAsync(start, end));
    }

    /// <summary>
    /// Downloads full order info as an Excel file.
    /// </summary>
    [HttpGet("{orderId:guid}/excel")]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DownloadExcel(Guid orderId)
    {
        var result = await _orderExcelExport.ExportOrderInfoAsync(orderId);
        if (result is null)
            return NotFound();

        return File(
            result.Value.Content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            result.Value.FileName);
    }


    /// <summary>
    /// Changes the status of an order.
    /// </summary>
    [HttpPatch("{orderId:guid}/status")]
    [Authorize(Roles = "Supervisor,Accountant")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ChangeStatus(Guid orderId, [FromQuery] OrderStatus status)
    {
        var success = await _orders.ChangeStatusAsync(orderId, status);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Permanently deletes an order and all associated data.
    /// </summary>
    [HttpDelete("{orderId:guid}")]
    [Authorize(Roles = "Supervisor")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid orderId)
    {
        var success = await _orders.DeleteOrderAsync(orderId);
        return success ? NoContent() : NotFound();
    }


}
