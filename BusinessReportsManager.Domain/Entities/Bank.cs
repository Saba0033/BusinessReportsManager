namespace BusinessReportsManager.Domain.Entities;

public class Bank : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public string? Swift { get; set; }
    
    public string? AccountNumber { get; set; }
}