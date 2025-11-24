namespace BusinessReportsManager.Application.DTOs.Passenger;

public class PassengerCreateDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
    public string? DocumentNumber { get; set; }
}