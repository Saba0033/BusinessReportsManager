namespace BusinessReportsManager.Domain.Entities;

public class Passenger : BaseEntity
{
    public Guid TourId { get; set; }
    
    public Tour? Tour { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    public DateOnly? BirthDate { get; set; }
    
    public string? DocumentNumber { get; set; }
}