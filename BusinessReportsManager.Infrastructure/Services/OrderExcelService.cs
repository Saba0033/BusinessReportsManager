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
            "Rent name",
            "Number of pax in",
            "List of passengers",
            "Order creation date",
            "Manager name",
            "Tour name",
            "Start date",
            "End date",
            "Gross price",
            "Ticket NI",
            "Ticket supplier",
            "Hotel NE",
            "Hotel supplier",
            "Transfer",
            "Transfer supplier",
            "Insurance",
            "Insurance supplier",
            "Other service",
            "Other service supplier",
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

            ws.Cell(row, 1).Value = o.RentName;
            ws.Cell(row, 2).Value = o.NumberOfPax;
            ws.Cell(row, 3).Value = o.ListOfPassengers;
            ws.Cell(row, 4).Value = o.OrderCreationDate.ToString("M/d/yyyy");
            ws.Cell(row, 5).Value = o.ManagerName;
            ws.Cell(row, 6).Value = o.TourName;
            ws.Cell(row, 7).Value = o.StartDate.ToString("M/d/yyyy");
            ws.Cell(row, 8).Value = o.EndDate.ToString("M/d/yyyy");
            ws.Cell(row, 9).Value = o.GrossPrice;
            ws.Cell(row, 10).Value = o.TicketPrice;
            ws.Cell(row, 11).Value = o.TicketSupplier;
            ws.Cell(row, 12).Value = o.HotelPrice;
            ws.Cell(row, 13).Value = o.HotelSupplier;
            ws.Cell(row, 14).Value = o.TransferPrice;
            ws.Cell(row, 15).Value = o.TransferSupplier;
            ws.Cell(row, 16).Value = o.InsurancePrice;
            ws.Cell(row, 17).Value = o.InsuranceSupplier;
            ws.Cell(row, 18).Value = o.OtherServicePrice;
            ws.Cell(row, 19).Value = o.OtherServiceSupplier;
            ws.Cell(row, 20).Value = o.Profit;
            ws.Cell(row, 21).Value = o.PaidByClient;
            ws.Cell(row, 22).Value = o.LeftToPay;
            ws.Cell(row, 23).Value = o.Currency;

            for (var c = 9; c <= 22; c++)
            {
                if (c == 11 || c == 13 || c == 15 || c == 17 || c == 19 || c == 23)
                    continue;
                ws.Cell(row, c).Style.NumberFormat.Format = "#,##0.00";
            }
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
