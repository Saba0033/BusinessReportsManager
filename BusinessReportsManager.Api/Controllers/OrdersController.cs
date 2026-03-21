using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Application.DTOs.Order;
using BusinessReportsManager.Application.DTOs.Payment;
using BusinessReportsManager.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BusinessReportsManager.API.Controllers;

/// <summary>
/// Manages orders including creation, editing, querying, financial reporting, and Excel export.
/// Each order contains party info, tour details, net prices per service category
/// (ticket, hotel, transfer, insurance, other), supplier names, payments, and accounting data.
/// </summary>
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
    /// Creates a new order with party, tour, passengers, tickets, hotel bookings,
    /// net prices (ticket, hotel, transfer, insurance, other service) with their suppliers,
    /// tour type, manager name, order source, customer bank requisites, and accounting data.
    /// A sequential integer OrderNumber is assigned automatically.
    /// </summary>
    /// <param name="dto">Order creation payload including all nested objects.</param>
    /// <returns>The created order with computed fields (PaidByClient, LeftToPay, Profit).</returns>
    /// <response code="201">Order created successfully.</response>
    /// <response code="500">Internal server error during creation.</response>
    [HttpPost]
    [Authorize(Roles = "Supervisor,Employee")]
    [ProducesResponseType(typeof(OrderDto), 201)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
    {
        try
        {
            var result = await _orders.CreateFullOrderAsync(dto);
            return CreatedAtAction(nameof(GetById), new { orderId = result.Id }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = ex.Message,
                details = ex.ToString()
            });
        }
    }

    /// <summary>
    /// Updates an existing order by fully replacing its nested structure,
    /// including passengers, tickets, hotel bookings, net prices, suppliers,
    /// tour type, manager name, and order source.
    /// </summary>
    /// <param name="orderId">The GUID of the order to update.</param>
    /// <param name="dto">The full replacement payload.</param>
    /// <returns>The updated order.</returns>
    /// <response code="200">Order updated successfully.</response>
    /// <response code="404">Order not found.</response>
    [HttpPut("{orderId:guid}")]
    [ProducesResponseType(typeof(OrderDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Edit(Guid orderId, [FromBody] OrderEditDto dto)
    {
        var result = await _orders.EditOrderAsync(orderId, dto);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Retrieves all orders as a flat report. Each row includes:
    /// OrderNo, ClientName, NumberOfPax, ListOfPassengers, OrderCreationDate,
    /// ManagerName, TourName, TourType, StartDate, EndDate, GrossPrice,
    /// TicketNet/Supplier, HotelNet/Supplier, TransferNet/Supplier,
    /// InsuranceNet/Supplier, OtherServiceNet/Supplier, Profit, PaidByClient, LeftToPay, Currency.
    /// </summary>
    /// <returns>List of flat order report records.</returns>
    /// <response code="200">Returns the list of order reports.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderReportDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _orders.GetAllReportAsync());
    }

    /// <summary>
    /// Retrieves a single order report by ID. Returns the same flat structure as GetAll
    /// including all net prices, suppliers, tour type, manager name, and financial totals.
    /// </summary>
    /// <param name="orderId">The GUID of the order.</param>
    /// <returns>A single flat order report.</returns>
    /// <response code="200">Returns the order report.</response>
    /// <response code="404">Order not found.</response>
    [HttpGet("{orderId:guid}")]
    [ProducesResponseType(typeof(OrderReportDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid orderId)
    {
        var result = await _orders.GetByIdReportAsync(orderId);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Retrieves all orders with a specific status (Open or Finalized) as flat reports.
    /// </summary>
    /// <param name="status">Order status filter (0 = Open, 1 = Finalized).</param>
    /// <returns>List of matching order reports.</returns>
    /// <response code="200">Returns the filtered list.</response>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(List<OrderReportDto>), 200)]
    public async Task<IActionResult> GetByStatus(OrderStatus status)
    {
        return Ok(await _orders.GetReportByStatusAsync(status));
    }

    /// <summary>
    /// Retrieves all orders associated with a specific party (customer) as flat reports.
    /// </summary>
    /// <param name="partyId">The GUID of the order party.</param>
    /// <returns>List of matching order reports.</returns>
    /// <response code="200">Returns the filtered list.</response>
    [HttpGet("party/{partyId:guid}")]
    [ProducesResponseType(typeof(List<OrderReportDto>), 200)]
    public async Task<IActionResult> GetByParty(Guid partyId)
    {
        return Ok(await _orders.GetReportByPartyAsync(partyId));
    }

    /// <summary>
    /// Retrieves all orders created within a date range as flat reports.
    /// </summary>
    /// <param name="start">Start of the date range (inclusive).</param>
    /// <param name="end">End of the date range (inclusive).</param>
    /// <returns>List of matching order reports.</returns>
    /// <response code="200">Returns the filtered list.</response>
    [HttpGet("date-range")]
    [ProducesResponseType(typeof(List<OrderReportDto>), 200)]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        return Ok(await _orders.GetReportByDateRangeAsync(start, end));
    }

    /// <summary>
    /// Searches orders by tour name (partial match) and/or date range. Returns flat reports.
    /// </summary>
    /// <param name="tourName">Optional partial tour name to search for (case-insensitive).</param>
    /// <param name="startDate">Optional minimum tour start date.</param>
    /// <param name="endDate">Optional maximum tour end date.</param>
    /// <returns>List of matching order reports.</returns>
    /// <response code="200">Returns the search results.</response>
    [HttpGet("search")]
    [Authorize(Roles = "Supervisor,Employee")]
    [ProducesResponseType(typeof(List<OrderReportDto>), 200)]
    public async Task<IActionResult> Search(
    [FromQuery] string? tourName,
    [FromQuery] DateOnly? startDate,
    [FromQuery] DateOnly? endDate)
    {
        return Ok(await _orders.SearchReportAsync(tourName, startDate, endDate));
    }


    /// <summary>
    /// Changes the status of an order (Open or Finalized).
    /// </summary>
    /// <param name="orderId">The GUID of the order.</param>
    /// <param name="status">The new status to set (0 = Open, 1 = Finalized).</param>
    /// <response code="204">Status changed successfully.</response>
    /// <response code="404">Order not found.</response>
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
    /// Permanently deletes an order and all associated data (tour, payments, etc.).
    /// </summary>
    /// <param name="orderId">The GUID of the order to delete.</param>
    /// <response code="204">Order deleted successfully.</response>
    /// <response code="404">Order not found.</response>
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
    /// Downloads all orders as an Excel (.xlsx) report. Columns match the standard report format:
    /// Order No, Client name, Number of pax, List of passengers, Order creation date,
    /// Manager name, Tour name, Start/End dates, Gross price,
    /// Ticket/Hotel/Transfer/Insurance/OtherService NET and suppliers,
    /// Profit, Paid by client, Left to pay, Currency.
    /// </summary>
    /// <returns>An Excel file download.</returns>
    /// <response code="200">Returns the Excel file.</response>
    [HttpGet("export-excel")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> ExportExcel()
    {
        var reportData = await _orders.GetAllReportAsync();
        var fileBytes = _orderExcel.GenerateReportExcel(reportData);
        return File(fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Report sample_{Guid.NewGuid()}.xlsx");
    }

    /// <summary>
    /// Downloads a single order as an Excel (.xlsx) report by order ID.
    /// Uses the same column format as the bulk export.
    /// </summary>
    /// <param name="orderId">The GUID of the order to export.</param>
    /// <returns>An Excel file download for the single order.</returns>
    /// <response code="200">Returns the Excel file.</response>
    /// <response code="404">Order not found.</response>
    [HttpGet("export-excel/{orderId:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ExportExcelById(Guid orderId)
    {
        var report = await _orders.GetByIdReportAsync(orderId);
        if (report is null)
            return NotFound();

        var fileBytes = _orderExcel.GenerateReportExcel(new List<OrderReportDto> { report });
        return File(fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Order_{report.OrderNo}_{Guid.NewGuid()}.xlsx");
    }

    /// <summary>
    /// Updates the accountant's comment on an order. Only Accountant and Supervisor roles can use this.
    /// The comment, timestamp, and updating user info are stored.
    /// </summary>
    /// <param name="orderId">The GUID of the order.</param>
    /// <param name="dto">The comment payload.</param>
    /// <response code="204">Comment updated successfully.</response>
    /// <response code="404">Order not found.</response>
    [HttpPatch("{orderId:guid}/accounting-comment")]
    [Authorize(Roles = "Accountant,Supervisor")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateAccountingComment(Guid orderId, [FromBody] AccountingCommentUpdateDto dto)
    {
        var ok = await _orders.UpdateAccountingCommentAsync(orderId, dto.Comment);
        return ok ? NoContent() : NotFound();
    }

    /// <summary>
    /// Returns a list of "golden" / saved customers — customers who have at least 2 orders.
    /// These can be used in the frontend for quick selection when creating new orders.
    /// </summary>
    /// <returns>List of saved customer IDs and full names.</returns>
    /// <response code="200">Returns the saved customers list.</response>
    [HttpGet("saved-customers")]
    [Authorize(Roles = "Supervisor,Employee")]
    [ProducesResponseType(typeof(List<SavedCustomerDto>), 200)]
    public async Task<IActionResult> GetSavedCustomers()
    {
        return Ok(await _orders.GetSavedCustomersAsync());
    }
}
