namespace BusinessReportsManager.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }
    
    public Order? Order { get; set; }

    public Guid PriceCurrencyId { get; set; }
    
    public PriceCurrency? PriceCurrency { get; set; }
    
    public string? BankName { get; set; }
    
    public DateOnly PaidDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    
    public string? Reference { get; set; }
}