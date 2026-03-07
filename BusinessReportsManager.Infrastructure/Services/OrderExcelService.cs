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
            "Number of pax in",
            "List of passengers",
            "Order creation date",
            "Start date",
            "End date",
            "Gross price",
            "Total expenses",
            "Profit",
            "Paid by client",
            "Left to pay",
            "Currency"
        ];

        for (var c = 0; c < headers.Length; c++)
        {
            ws.Cell(1, c + 1).Value = headers[c];
            ws.Cell(1, c + 1).Style.Font.Bold = true;
        }

        for (var i = 0; i < orders.Count; i++)
        {
            var o = orders[i];
            var row = i + 2;

            ws.Cell(row, 1).Value = o.NumberOfPax;
            ws.Cell(row, 2).Value = o.ListOfPassengers;
            ws.Cell(row, 3).Value = o.OrderCreationDate.ToString("M/d/yyyy");
            ws.Cell(row, 4).Value = o.StartDate.ToString("M/d/yyyy");
            ws.Cell(row, 5).Value = o.EndDate.ToString("M/d/yyyy");
            ws.Cell(row, 6).Value = o.GrossPrice;
            ws.Cell(row, 7).Value = o.TotalExpenses;
            ws.Cell(row, 8).Value = o.Profit;
            ws.Cell(row, 9).Value = o.PaidByClient;
            ws.Cell(row, 10).Value = o.LeftToPay;
            ws.Cell(row, 11).Value = o.Currency;

            for (var c = 6; c <= 10; c++)
                ws.Cell(row, c).Style.NumberFormat.Format = "#,##0.00";
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
