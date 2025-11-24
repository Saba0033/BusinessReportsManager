namespace BusinessReportsManager.Application.DTOs.Supplier;

public class SupplierCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
    public string? Phone { get; set; }
}