namespace BusinessReportsManager.Application.AbstractServices;

public interface IOrderExcelService
{
    Task<byte[]> GenerateOrderExcelAsync(Guid orderId);
}
