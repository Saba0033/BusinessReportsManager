using BusinessReportsManager.Domain.ValueObjects;

namespace BusinessReportsManager.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }
    
    public Order? Order { get; set; }
    
    public Money Amount { get; set; } = new Money(0, Enums.Currency.GEL);
    
    public Guid BankId { get; set; }
    
    public Bank? Bank { get; set; }
    
    public DateOnly PaidDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    
    public string? Reference { get; set; }
}