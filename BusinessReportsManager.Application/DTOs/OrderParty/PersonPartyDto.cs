namespace BusinessReportsManager.Application.DTOs.OrderParty;

public class PersonPartyDto
{
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
}