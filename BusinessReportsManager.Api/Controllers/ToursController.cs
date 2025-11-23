using BusinessReportsManager.Application.AbstractServices;
using Microsoft.AspNetCore.Mvc;

namespace BusinessReportsManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToursController : ControllerBase
{
    private readonly ITourService _service;

    public ToursController(ITourService service)
    {
        _service = service;
    }

    // ---------------------------
    // Get All Tours
    // ---------------------------
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tours = await _service.GetAllToursAsync();
        return Ok(tours);
    }

    // ---------------------------
    // Get Tour by Id
    // ---------------------------
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var tour = await _service.GetTourAsync(id);
        if (tour == null) return NotFound();

        return Ok(tour);
    }

    // ---------------------------
    // Create Tour
    // ---------------------------
    public class CreateTourDto
    {
        public string Name { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int PassengerCount { get; set; }
        public Guid SupplierId { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTourDto dto)
    {
        var tour = await _service.CreateTourAsync(
            dto.Name,
            dto.StartDate,
            dto.EndDate,
            dto.PassengerCount,
            dto.SupplierId
        );

        return CreatedAtAction(nameof(Get), new { id = tour.Id }, tour);
    }

    // ---------------------------
    // Delete Tour
    // ---------------------------
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _service.DeleteTourAsync(id);

        if (!success) return NotFound();
        return NoContent();
    }
}
