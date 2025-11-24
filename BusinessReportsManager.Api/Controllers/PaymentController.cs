using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs.Payment;
using Microsoft.AspNetCore.Mvc;

namespace BusinessReportsManager.API.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _payments;

    public PaymentController(IPaymentService payments)
    {
        _payments = payments;
    }

    // ====================================================
    // ADD PAYMENT
    // ====================================================
    /// <summary>
    /// Adds a payment made by the customer for the specified order.
    /// </summary>
    [HttpPost("{orderId:guid}")]
    [ProducesResponseType(typeof(PaymentDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddPayment(Guid orderId, [FromBody] PaymentCreateDto dto)
    {
        var result = await _payments.AddPaymentAsync(orderId, dto);
        return result is null ? NotFound() : Ok(result);
    }

    // ====================================================
    // REMOVE PAYMENT
    // ====================================================
    /// <summary>
    /// Removes a specific payment from the system.
    /// </summary>
    [HttpDelete("payment/{paymentId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RemovePayment(Guid paymentId)
    {
        var success = await _payments.RemovePaymentAsync(paymentId);
        return success ? NoContent() : NotFound();
    }

    // ====================================================
    // TOTAL CUSTOMER PAID
    // ====================================================
    /// <summary>
    /// Returns the total amount paid by the customer for the order (converted to GEL).
    /// </summary>
    [HttpGet("{orderId:guid}/customer-paid")]
    [ProducesResponseType(typeof(decimal), 200)]
    public async Task<IActionResult> GetCustomerPaid(Guid orderId)
    {
        var total = await _payments.GetTotalPaidAsync(orderId);
        return Ok(total);
    }

    // ====================================================
    // TOTAL EXPENSES
    // ====================================================
    /// <summary>
    /// Returns total supplier expenses for the order (Air Tickets + Hotel Bookings + Extra Services),
    /// automatically converted to GEL.
    /// </summary>
    [HttpGet("{orderId:guid}/expenses")]
    [ProducesResponseType(typeof(decimal), 200)]
    public async Task<IActionResult> GetExpenses(Guid orderId)
    {
        var expenses = await _payments.GetExpensesAsync(orderId);
        return Ok(expenses);
    }

    // ====================================================
    // PROFIT
    // ====================================================
    /// <summary>
    /// Returns the profit of the order (SellPrice - Expenses).
    /// </summary>
    [HttpGet("{orderId:guid}/profit")]
    [ProducesResponseType(typeof(decimal), 200)]
    public async Task<IActionResult> GetProfit(Guid orderId)
    {
        var profit = await _payments.GetProfitAsync(orderId);
        return Ok(profit);
    }

    // ====================================================
    // SUPPLIER OWED
    // ====================================================
    /// <summary>
    /// Returns the amount owed to suppliers for the order.
    /// Currently equal to the total expenses (Air + Hotel + Extra).
    /// </summary>
    [HttpGet("{orderId:guid}/supplier-owed")]
    [ProducesResponseType(typeof(decimal), 200)]
    public async Task<IActionResult> GetSupplierOwed(Guid orderId)
    {
        var owed = await _payments.GetSupplierOwedAsync(orderId);
        return Ok(owed);
    }
}
