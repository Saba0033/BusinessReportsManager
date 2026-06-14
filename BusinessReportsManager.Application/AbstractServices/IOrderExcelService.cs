using BusinessReportsManager.Application.DTOs.Order;

namespace BusinessReportsManager.Application.AbstractServices;

public interface IOrderExcelService
{
    byte[] GenerateReportExcel(List<OrderReportDto> orders, string sheetName = "Report sample");

    /// <summary>
    /// Generates a customer-facing invoice (.xlsx) for a single order.
    /// Shows the gross/sell price, amounts paid and the outstanding balance —
    /// internal NET costs, suppliers and profit are intentionally excluded.
    /// </summary>
    byte[] GenerateInvoiceExcel(OrderDto order);
}
