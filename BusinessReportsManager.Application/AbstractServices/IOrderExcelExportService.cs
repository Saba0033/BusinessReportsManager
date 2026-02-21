namespace BusinessReportsManager.Application.AbstractServices;

public interface IOrderExcelExportService
{
    Task<(byte[] Content, string FileName)?> ExportOrderInfoAsync(Guid orderId);
}
