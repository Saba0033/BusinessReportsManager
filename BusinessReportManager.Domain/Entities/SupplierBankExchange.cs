using BusinessReportsManager.Domain.Common;

namespace BusinessReportsManager.Domain.Entities;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

public class Bank
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ExchangeRate
{
    public int Id { get; set; }
    public Currency Currency { get; set; }
    /// <summary>
    /// Rate to convert 1 unit of Currency to GEL (Georgian Lari)
    /// </summary>
    public decimal RateToGel { get; set; }
    public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
}