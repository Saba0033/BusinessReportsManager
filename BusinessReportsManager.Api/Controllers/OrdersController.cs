using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Application.DTOs.Order;
using BusinessReportsManager.Application.DTOs.Payment;
using BusinessReportsManager.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BusinessReportsManager.API.Controllers;

[Authorize]
[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orders;
    private readonly IPaymentService _payments;
    private readonly IOrderExcelService _orderExcel;

    public OrderController(IOrderService orders, IPaymentService payments, IOrderExcelService orderExcel)
    {
        _orders = orders;
        _payments = payments;
        _orderExcel = orderExcel;
    }

    /// <summary>
    /// Creates a new order with party, tour, passengers, services, and payments.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Supervisor,Employee")]
    [ProducesResponseType(typeof(OrderDto), 201)]
    public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
    {
        try
        {
            var result = await _orders.CreateFullOrderAsync(dto);
            return CreatedAtAction(nameof(GetById), new { orderId = result.Id }, result);
        }
        catch (Exception ex)
        {
            // TEMPORARY for debugging only (remove later)
            return StatusCode(500, new
            {
                error = ex.Message,
                details = ex.ToString()
            });
        }
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

    [HttpGet("search")]
    [Authorize(Roles = "Supervisor,Employee")]
    [ProducesResponseType(typeof(List<OrderDto>), 200)]
    public async Task<IActionResult> Search(
    [FromQuery] string? tourName,
    [FromQuery] DateOnly? startDate,
    [FromQuery] DateOnly? endDate)
    {
        return Ok(await _orders.SearchAsync(tourName, startDate, endDate));
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

    /// <summary>
    /// Downloads the order information as an Excel file.
    /// </summary>
    [HttpGet("{orderId:guid}/export-excel")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ExportExcel(Guid orderId)
    {
        try
        {
            var fileBytes = await _orderExcel.GenerateOrderExcelAsync(orderId);
            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"order-{orderId}.xlsx");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPatch("{orderId:guid}/accounting-comment")]
    [Authorize(Roles = "Accountant,Supervisor")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateAccountingComment(Guid orderId, [FromBody] AccountingCommentUpdateDto dto)
    {
        var ok = await _orders.UpdateAccountingCommentAsync(orderId, dto.Comment);
        return ok ? NoContent() : NotFound();
    }

    [HttpGet("saved-customers")]
    [Authorize(Roles = "Supervisor,Employee")]
    [ProducesResponseType(typeof(List<SavedCustomerDto>), 200)]
    public async Task<IActionResult> GetSavedCustomers()
    {
        return Ok(await _orders.GetSavedCustomersAsync());
    }
}
