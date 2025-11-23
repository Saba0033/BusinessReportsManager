using BusinessReportsManager.Application.AbstractServices;
using Microsoft.AspNetCore.Mvc;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Domain.Enums;
using BusinessReportsManager.Domain.Interfaces;

namespace BusinessReportsManager.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _service;

    public OrderController(IOrderService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(OrderStatus status) =>
        Ok(await _service.GetByStatusAsync(status));

    [HttpGet("party/{partyId:guid}")]
    public async Task<IActionResult> GetByParty(Guid partyId) =>
        Ok(await _service.GetByPartyAsync(partyId));

    [HttpGet("dates")]
    public async Task<IActionResult> GetByDates(DateTime start, DateTime end) =>
        Ok(await _service.GetByDateRangeAsync(start, end));

    [HttpPost]
    public async Task<IActionResult> Create(OrderCreateDto dto) =>
        Ok(await _service.CreateFullOrderAsync(dto));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, OrderEditDto dto)
    {
        var result = await _service.EditOrderAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, OrderStatus status)
    {
        var success = await _service.ChangeStatusAsync(id, status);
        return success ? Ok() : NotFound();
    }

    [HttpPost("{id:guid}/payment")]
    public async Task<IActionResult> AddPayment(Guid id, PaymentCreateDto dto)
    {
        var payment = await _service.AddPaymentAsync(id, dto);
        return payment == null ? NotFound() : Ok(payment);
    }

    [HttpDelete("payment/{paymentId:guid}")]
    public async Task<IActionResult> RemovePayment(Guid paymentId)
    {
        var result = await _service.RemovePaymentAsync(paymentId);
        return result ? Ok() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteOrderAsync(id);
        return result ? Ok() : NotFound();
    }
}
