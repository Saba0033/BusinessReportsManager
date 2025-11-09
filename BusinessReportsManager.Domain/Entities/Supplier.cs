namespace BusinessReportsManager.Domain.Entities;

public class Supplier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public string? TaxId { get; set; }
    
    public string? ContactEmail { get; set; }
    
    public string? Phone { get; set; }
    
    public ICollection<Tour> Tours{ get; set; } = new List<Tour>();
    
}