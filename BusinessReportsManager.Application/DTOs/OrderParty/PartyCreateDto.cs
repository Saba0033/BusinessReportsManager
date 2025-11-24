namespace BusinessReportsManager.Application.DTOs.OrderParty;

public class PartyCreateDto
{
    public string Type { get; set; } = "Person";   // "Person" or "Company"

    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    // Person fields
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly? BirthDate { get; set; }

    // Company fields
    public string? CompanyName { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? ContactPerson { get; set; }
}