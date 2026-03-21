using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs.Order;
using ClosedXML.Excel;

namespace BusinessReportsManager.Infrastructure.Services;

public class OrderExcelService : IOrderExcelService
{
    public byte[] GenerateReportExcel(List<OrderReportDto> orders, string sheetName = "Report sample")
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add(sheetName);

        string[] headers =
        [
            "Order No",
            "Client name",
            "Number of pax in booking",
            "List of passengers",
            "Order creation date",
            "Manager name",
            "Tour name",
            "Start date",
            "End date",
            "Gross price",
            "Ticket NET",
            "Ticket supplier",
            "Hotel NET",
            "Hotel supplier",
            "Transfer NET",
            "Transfer supplier",
            "Insurance NET",
            "Insurance supplier",
            "Other service NET",
            "Other service supplier",
            "Profit",
            "Paid by client",
            "Left to pay",
            "Currency"
        ];

        var headerStyle = ws.Range(1, 1, 1, headers.Length);
        headerStyle.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFD700");
        headerStyle.Style.Font.Bold = true;

        for (var c = 0; c < headers.Length; c++)
        {
            ws.Cell(1, c + 1).Value = headers[c];
        }

        for (var i = 0; i < orders.Count; i++)
        {
            var o = orders[i];
            var row = i + 2;

            ws.Cell(row, 1).Value = o.OrderNo;
            ws.Cell(row, 2).Value = o.ClientName;
            ws.Cell(row, 3).Value = o.NumberOfPax;
            ws.Cell(row, 4).Value = o.ListOfPassengers;
            ws.Cell(row, 5).Value = o.OrderCreationDate.ToString("M/d/yyyy");
            ws.Cell(row, 6).Value = o.ManagerName ?? string.Empty;
            ws.Cell(row, 7).Value = o.TourName ?? string.Empty;
            ws.Cell(row, 8).Value = o.StartDate.ToString("M/d/yyyy");
            ws.Cell(row, 9).Value = o.EndDate.ToString("M/d/yyyy");
            ws.Cell(row, 10).Value = o.GrossPrice;
            ws.Cell(row, 11).Value = o.TicketNet;
            ws.Cell(row, 12).Value = o.TicketSupplier ?? string.Empty;
            ws.Cell(row, 13).Value = o.HotelNet;
            ws.Cell(row, 14).Value = o.HotelSupplier ?? string.Empty;
            ws.Cell(row, 15).Value = o.TransferNet;
            ws.Cell(row, 16).Value = o.TransferSupplier ?? string.Empty;
            ws.Cell(row, 17).Value = o.InsuranceNet;
            ws.Cell(row, 18).Value = o.InsuranceSupplier ?? string.Empty;
            ws.Cell(row, 19).Value = o.OtherServiceNet;
            ws.Cell(row, 20).Value = o.OtherServiceSupplier ?? string.Empty;
            ws.Cell(row, 21).Value = o.Profit;
            ws.Cell(row, 22).Value = o.PaidByClient;
            ws.Cell(row, 23).Value = o.LeftToPay;
            ws.Cell(row, 24).Value = o.Currency;

            foreach (var c in new[] { 10, 11, 13, 15, 17, 19, 21, 22, 23 })
                ws.Cell(row, c).Style.NumberFormat.Format = "#,##0.00";
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
