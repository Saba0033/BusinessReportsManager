using BusinessReportsManager.Application.AbstractServices;
using Microsoft.AspNetCore.Mvc;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace BusinessReportsManager.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize] // Global auth
public class OrderController : ControllerBase
{
    private readonly IOrderService _service;

    public OrderController(IOrderService service)
    {
        _service = service;
    }

    // ==============================
    // GET ALL — Supervisor sees all; others see own
    // ==============================
    [Authorize(Roles = "Supervisor,Accountant,Employee")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync(User));

    // ==============================
    // GET BY ID — Supervisor sees any; others see own
    // ==============================
    [Authorize(Roles = "Supervisor,Accountant,Employee")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _service.GetByIdAsync(id, User);
        return result == null ? NotFound() : Ok(result);
    }

    // ==============================
    // GET BY STATUS
    // ==============================
    [Authorize(Roles = "Supervisor,Accountant,Employee")]
    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(OrderStatus status)
        => Ok(await _service.GetByStatusAsync(status, User));

    // ==============================
    // GET BY PARTY
    // ==============================
    [Authorize(Roles = "Supervisor,Accountant,Employee")]
    [HttpGet("party/{partyId:guid}")]
    public async Task<IActionResult> GetByParty(Guid partyId)
        => Ok(await _service.GetByPartyAsync(partyId, User));

    // ==============================
    // GET BY DATE RANGE
    // ==============================
    [Authorize(Roles = "Supervisor,Accountant,Employee")]
    [HttpGet("dates")]
    public async Task<IActionResult> GetByDates(DateTime start, DateTime end)
        => Ok(await _service.GetByDateRangeAsync(start, end, User));

    // ==============================
    // CREATE ORDER
    // Only EMPLOYEE and SUPERVISOR (NOT Accountant)
    // ==============================
    [Authorize(Roles = "Supervisor, Employee")]
    [HttpPost]
    public async Task<IActionResult> Create(OrderCreateDto dto)
        => Ok(await _service.CreateFullOrderAsync(dto, User));

    // ==============================
    // EDIT ORDER
    // ALL roles allowed — service enforces business rules
    // ==============================
    [Authorize(Roles = "Supervisor,Accountant,Employee")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, OrderEditDto dto)
    {
        var result = await _service.EditOrderAsync(id, dto, User);
        return result == null ? NotFound() : Ok(result);
    }

    // ==============================
    // CHANGE STATUS — Accountant OR Supervisor
    // ==============================
    [Authorize(Roles = "Supervisor,Accountant")]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, OrderStatus status)
    {
        var success = await _service.ChangeStatusAsync(id, status, User);
        return success ? Ok() : NotFound();
    }

    // ==============================
    // ADD PAYMENT — Accountant OR Supervisor
    // ==============================
    [Authorize(Roles = "Supervisor,Accountant")]
    [HttpPost("{id:guid}/payment")]
    public async Task<IActionResult> AddPayment(Guid id, PaymentCreateDto dto)
    {
        var payment = await _service.AddPaymentAsync(id, dto, User);
        return payment == null ? NotFound() : Ok(payment);
    }

    // ==============================
    // REMOVE PAYMENT — Accountant OR Supervisor
    // ==============================
    [Authorize(Roles = "Supervisor,Accountant")]
    [HttpDelete("payment/{paymentId:guid}")]
    public async Task<IActionResult> RemovePayment(Guid paymentId)
    {
        var result = await _service.RemovePaymentAsync(paymentId, User);
        return result ? Ok() : NotFound();
    }

    // ==============================
    // DELETE ORDER — Accountant OR Supervisor
    // ==============================
    [Authorize(Roles = "Supervisor,Accountant")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteOrderAsync(id, User);
        return result ? Ok() : NotFound();
    }
}
