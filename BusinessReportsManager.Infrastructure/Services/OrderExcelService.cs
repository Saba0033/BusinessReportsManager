using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs.Order;
using ClosedXML.Excel;

namespace BusinessReportsManager.Infrastructure.Services;

public class OrderExcelService : IOrderExcelService
{
    // Customer-facing company header. Adjust to the agency's real details.
    private const string CompanyName = "Tour Agency";
    private const string CompanyContact = "Tbilisi, Georgia · info@touragency.ge · +995 32 000 0000";

    public byte[] GenerateInvoiceExcel(OrderDto order)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add($"Invoice {order.OrderNumber}");

        ws.Column(1).Width = 28;
        ws.Column(2).Width = 18;
        ws.Column(3).Width = 14;
        ws.Column(4).Width = 18;

        var accent = XLColor.FromHtml("#1F4E78");

        // ---- Company header ----
        ws.Range(1, 1, 1, 4).Merge();
        ws.Cell(1, 1).Value = CompanyName;
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 18;
        ws.Cell(1, 1).Style.Font.FontColor = accent;

        ws.Range(2, 1, 2, 4).Merge();
        ws.Cell(2, 1).Value = CompanyContact;
        ws.Cell(2, 1).Style.Font.FontColor = XLColor.Gray;

        // ---- Title ----
        ws.Range(4, 1, 4, 4).Merge();
        ws.Cell(4, 1).Value = "INVOICE";
        ws.Cell(4, 1).Style.Font.Bold = true;
        ws.Cell(4, 1).Style.Font.FontSize = 22;

        ws.Cell(5, 1).Value = "Invoice No:";
        ws.Cell(5, 1).Style.Font.Bold = true;
        ws.Cell(5, 2).Value = $"INV-{order.OrderNumber:D5}";
        ws.Cell(6, 1).Value = "Issue date:";
        ws.Cell(6, 1).Style.Font.Bold = true;
        ws.Cell(6, 2).Value = DateTime.UtcNow.ToString("dd MMM yyyy");

        // ---- Bill to ----
        ws.Cell(8, 1).Value = "BILL TO";
        ws.Cell(8, 1).Style.Font.Bold = true;
        ws.Cell(8, 1).Style.Font.FontColor = accent;
        ws.Cell(9, 1).Value = order.Party.FullName;
        var billRow = 10;
        if (!string.IsNullOrWhiteSpace(order.Party.PersonalNumber))
            ws.Cell(billRow++, 1).Value = $"ID/PN: {order.Party.PersonalNumber}";
        if (!string.IsNullOrWhiteSpace(order.Party.Email))
            ws.Cell(billRow++, 1).Value = order.Party.Email;
        if (!string.IsNullOrWhiteSpace(order.Party.Phone))
            ws.Cell(billRow++, 1).Value = order.Party.Phone;

        // ---- Line items table ----
        var headerRow = billRow + 1;
        string[] cols = ["Description", "Period", "Pax", "Amount (GEL)"];
        for (var c = 0; c < cols.Length; c++)
        {
            var cell = ws.Cell(headerRow, c + 1);
            cell.Value = cols[c];
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = accent;
        }

        var itemRow = headerRow + 1;
        var period = $"{order.Tour.StartDate:dd MMM yyyy} – {order.Tour.EndDate:dd MMM yyyy}";
        var description = string.IsNullOrWhiteSpace(order.Tour.Destination)
            ? "Tour package"
            : $"Tour package — {order.Tour.Destination}";
        ws.Cell(itemRow, 1).Value = description;
        ws.Cell(itemRow, 2).Value = period;
        ws.Cell(itemRow, 3).Value = order.Tour.PassengerCount;
        ws.Cell(itemRow, 4).Value = order.SellPriceInGel;
        ws.Cell(itemRow, 4).Style.NumberFormat.Format = "#,##0.00";

        // ---- Totals ----
        var totalsRow = itemRow + 2;

        void Total(string label, decimal value, bool bold)
        {
            ws.Cell(totalsRow, 3).Value = label;
            ws.Cell(totalsRow, 3).Style.Font.Bold = bold;
            ws.Cell(totalsRow, 4).Value = value;
            ws.Cell(totalsRow, 4).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell(totalsRow, 4).Style.Font.Bold = bold;
            totalsRow++;
        }

        Total("Subtotal", order.SellPriceInGel, false);
        Total("Paid", order.PaidByClient, false);
        Total("Balance due", order.LeftToPay, true);

        ws.Range(totalsRow - 1, 3, totalsRow - 1, 4).Style.Border.TopBorder = XLBorderStyleValues.Thin;

        // ---- Payments breakdown (optional) ----
        if (order.Payments.Count > 0)
        {
            var payHeader = totalsRow + 1;
            ws.Cell(payHeader, 1).Value = "Payments received";
            ws.Cell(payHeader, 1).Style.Font.Bold = true;
            ws.Cell(payHeader, 1).Style.Font.FontColor = accent;

            var pRow = payHeader + 1;
            ws.Cell(pRow, 1).Value = "Date";
            ws.Cell(pRow, 2).Value = "Bank";
            ws.Cell(pRow, 4).Value = "Amount (GEL)";
            ws.Range(pRow, 1, pRow, 4).Style.Font.Bold = true;
            pRow++;

            foreach (var p in order.Payments)
            {
                ws.Cell(pRow, 1).Value = p.PaidDate.ToString("dd MMM yyyy");
                ws.Cell(pRow, 2).Value = p.BankName ?? string.Empty;
                ws.Cell(pRow, 4).Value = p.Price.PriceInGel ?? p.Price.Amount;
                ws.Cell(pRow, 4).Style.NumberFormat.Format = "#,##0.00";
                pRow++;
            }
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }


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
            "Cruise NET",
            "Cruise supplier",
            "Insurance NET",
            "Insurance supplier",
            "Other service NET",
            "Other service supplier",
            "Total expenses",
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

        var moneyColumns = new List<int>();

        for (var i = 0; i < orders.Count; i++)
        {
            var o = orders[i];
            var row = i + 2;
            var col = 1;

            void Text(string? v) => ws.Cell(row, col++).Value = v ?? string.Empty;
            void Num(decimal v) { moneyColumns.Add(col); ws.Cell(row, col++).Value = v; }

            ws.Cell(row, col++).Value = o.OrderNo;
            Text(o.ClientName);
            ws.Cell(row, col++).Value = o.NumberOfPax;
            Text(o.ListOfPassengers);
            Text(o.OrderCreationDate.ToString("M/d/yyyy"));
            Text(o.ManagerName);
            Text(o.TourName);
            Text(o.StartDate.ToString("M/d/yyyy"));
            Text(o.EndDate.ToString("M/d/yyyy"));
            Num(o.GrossPrice);
            Num(o.TicketNet);
            Text(o.TicketSupplier);
            Num(o.HotelNet);
            Text(o.HotelSupplier);
            Num(o.TransferNet);
            Text(o.TransferSupplier);
            Num(o.CruiseNet);
            Text(o.CruiseSupplier);
            Num(o.InsuranceNet);
            Text(o.InsuranceSupplier);
            Num(o.OtherServiceNet);
            Text(o.OtherServiceSupplier);
            Num(o.TotalExpenses);
            Num(o.Profit);
            Num(o.PaidByClient);
            Num(o.LeftToPay);
            Text(o.Currency);
        }

        foreach (var c in moneyColumns.Distinct())
            ws.Column(c).Style.NumberFormat.Format = "#,##0.00";

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
