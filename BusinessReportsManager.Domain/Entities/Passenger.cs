namespace BusinessReportsManager.Domain.Entities;

public class Passenger : BaseEntity
{
    public Guid OrderId { get; set; }
    
    public Order? Order { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    public bool IsPrimary { get; set; }

    public DateOnly? BirthDate { get; set; }
    
    public string? DocumentNumber { get; set; }
}