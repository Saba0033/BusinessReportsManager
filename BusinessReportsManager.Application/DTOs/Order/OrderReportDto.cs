namespace BusinessReportsManager.Application.DTOs.Order;

public class OrderReportDto
{
    public Guid Id { get; set; }
    public int OrderNo { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public int NumberOfPax { get; set; }
    public string ListOfPassengers { get; set; } = string.Empty;
    public DateTime OrderCreationDate { get; set; }
    public string? ManagerName { get; set; }
    public string? TourName { get; set; }
    public string? TourType { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal GrossPrice { get; set; }

    public decimal TicketNet { get; set; }
    public string? TicketSupplier { get; set; }
    public decimal HotelNet { get; set; }
    public string? HotelSupplier { get; set; }
    public decimal TransferNet { get; set; }
    public string? TransferSupplier { get; set; }
    public decimal InsuranceNet { get; set; }
    public string? InsuranceSupplier { get; set; }
    public decimal OtherServiceNet { get; set; }
    public string? OtherServiceSupplier { get; set; }

    public decimal Profit { get; set; }
    public decimal PaidByClient { get; set; }
    public decimal LeftToPay { get; set; }
    public string Currency { get; set; } = string.Empty;
}
