using BusinessReportsManager.Application.DTOs.Order;

namespace BusinessReportsManager.Application.AbstractServices;

public interface IOrderExcelService
{
    byte[] GenerateReportExcel(List<OrderReportDto> orders, string sheetName = "Report sample");
}
