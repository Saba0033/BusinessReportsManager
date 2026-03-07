namespace BusinessReportsManager.Application.DTOs.Order;

public class OrderReportDto
{
    public Guid Id { get; set; }
    public int NumberOfPax { get; set; }
    public string ListOfPassengers { get; set; } = string.Empty;
    public DateTime OrderCreationDate { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal GrossPrice { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Profit { get; set; }
    public decimal PaidByClient { get; set; }
    public decimal LeftToPay { get; set; }
    public string Currency { get; set; } = string.Empty;
}
