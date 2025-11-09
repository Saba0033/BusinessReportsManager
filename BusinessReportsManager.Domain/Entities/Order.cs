using BusinessReportsManager.Domain.Enums;
using BusinessReportsManager.Domain.ValueObjects;

namespace BusinessReportsManager.Domain.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public Guid OrderPartyId { get; set; }
    public OrderParty? OrderParty { get; set; }
    public Guid TourId { get; set; }
    public Tour? Tour { get; set; }
    public string Source { get; set; } = string.Empty;
    public string OwnedByUserId { get; set; } = string.Empty;

    public Money SellPrice { get; set; } = new Money(0, Currency.GEL);
    public Money TicketSelfCost { get; set; } = new Money(0, Currency.GEL);

    public OrderStatus Status { get; set; } = OrderStatus.Open;

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}