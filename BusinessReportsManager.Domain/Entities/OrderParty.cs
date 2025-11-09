namespace BusinessReportsManager.Domain.Entities;

public abstract class OrderParty : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    
    public string? Phone { get; set; }
    
}

public sealed class PersonParty : OrderParty
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
}

public sealed class CompanyParty : OrderParty
{
    public string CompanyName { get; set; } = string.Empty;
    public string? RegistrationNumber { get; set; }
    public string? ContactPerson { get; set; }
}