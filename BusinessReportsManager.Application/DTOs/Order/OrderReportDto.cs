namespace BusinessReportsManager.Application.DTOs.Order;

public class OrderReportDto
{
    public Guid Id { get; set; }
    public string RentName { get; set; } = string.Empty;
    public int NumberOfPax { get; set; }
    public string ListOfPassengers { get; set; } = string.Empty;
    public DateTime OrderCreationDate { get; set; }
    public string ManagerName { get; set; } = string.Empty;
    public string TourName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal GrossPrice { get; set; }
    public decimal TicketPrice { get; set; }
    public string TicketSupplier { get; set; } = string.Empty;
    public decimal HotelPrice { get; set; }
    public string HotelSupplier { get; set; } = string.Empty;
    public decimal TransferPrice { get; set; }
    public string TransferSupplier { get; set; } = string.Empty;
    public decimal InsurancePrice { get; set; }
    public string InsuranceSupplier { get; set; } = string.Empty;
    public decimal OtherServicePrice { get; set; }
    public string OtherServiceSupplier { get; set; } = string.Empty;
    public decimal Profit { get; set; }
    public decimal PaidByClient { get; set; }
    public decimal LeftToPay { get; set; }
    public string Currency { get; set; } = string.Empty;
}
