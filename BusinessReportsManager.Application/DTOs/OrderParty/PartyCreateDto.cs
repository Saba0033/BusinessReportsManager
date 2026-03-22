namespace BusinessReportsManager.Application.DTOs.OrderParty;

public class PartyCreateDto
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? PersonalNumber { get; set; }
}
