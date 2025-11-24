namespace BusinessReportsManager.Application.DTOs.OrderParty;

public class CompanyPartyDto
{
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    public string CompanyName { get; set; } = string.Empty;
    public string? RegistrationNumber { get; set; }
    public string? ContactPerson { get; set; }
}