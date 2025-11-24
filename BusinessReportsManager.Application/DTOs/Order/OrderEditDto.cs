using BusinessReportsManager.Application.DTOs.OrderParty;
using BusinessReportsManager.Application.DTOs.Passenger;
using BusinessReportsManager.Application.DTOs.Payment;
using BusinessReportsManager.Application.DTOs.Tour;

namespace BusinessReportsManager.Application.DTOs.Order;

public class OrderEditDto : OrderCreateDto
{
    public Guid OrderId { get; set; }
}