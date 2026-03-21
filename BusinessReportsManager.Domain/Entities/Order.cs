using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Domain.Entities;

public class Order : BaseEntity
{
    public int OrderNumber { get; set; }
    public Guid OrderPartyId { get; set; }
    public Guid? CreatedById { get; set; }
    public string? CreatedByEmail { get; set; } = string.Empty;
    public string? ManagerName { get; set; }
    public OrderParty? OrderParty { get; set; }
    public Guid TourId { get; set; }
    public Tour? Tour { get; set; }
    public string Source { get; set; } = string.Empty;
    public string? TourType { get; set; }
    public decimal SellPriceInGel { get; set; } = 0;
    public decimal TotalExpenseInGel { get; set; } = 0;

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

    public Guid? CustomerBankRequisitesId { get; set; }
    public CustomerBankRequisites? CustomerBankRequisites { get; set; }
    public string? AccountingComment { get; set; }
    public DateTime? AccountingCommentUpdatedAtUtc { get; set; }
    public Guid? AccountingCommentUpdatedById { get; set; }
    public string? AccountingCommentUpdatedByEmail { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Open;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}