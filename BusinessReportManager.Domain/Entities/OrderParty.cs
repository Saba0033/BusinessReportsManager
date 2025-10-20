namespace BusinessReportsManager.Domain.Entities;

public abstract class OrderParty
{
    public int Id { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public class PersonOrderParty : OrderParty
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string IdNumber { get; set; } = string.Empty;

    public void RefreshDisplayName()
    {
        DisplayName = $"{Name} {Surname}".Trim();
    }
}

public class CompanyOrderParty : OrderParty
{
    public string CompanyName { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;

    public void RefreshDisplayName()
    {
        DisplayName = CompanyName;
    }
}