using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Domain.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public Guid OrderPartyId { get; set; }
    public Guid? CreatedById { get; set; }
    public string? CreatedByEmail { get; set; } = string.Empty;
    public OrderParty? OrderParty { get; set; }
    public Guid TourId { get; set; }
    public Tour? Tour { get; set; }
    public string Source { get; set; } = string.Empty;
    public decimal SellPriceInGel { get; set; } = 0;
    public Guid? CustomerBankRequisitesId { get; set; }
    public CustomerBankRequisites? CustomerBankRequisites { get; set; }
    public string? AccountingComment { get; set; }
    public DateTime? AccountingCommentUpdatedAtUtc { get; set; }
    public Guid? AccountingCommentUpdatedById { get; set; }
    public string? AccountingCommentUpdatedByEmail { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Open;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}