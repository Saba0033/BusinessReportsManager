namespace BusinessReportsManager.Application.DTOs.OrderParty;

public class PartyCreateDto
{
    public Guid? Id { get; set; } // if existing party, FE can send it
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
