using BusinessReportsManager.Application.AbstractServices;
using ClosedXML.Excel;

namespace BusinessReportsManager.Infrastructure.Services;

public class OrderExcelExportService : IOrderExcelExportService
{
    private readonly IOrderService _orders;

    public OrderExcelExportService(IOrderService orders)
    {
        _orders = orders;
    }

    public async Task<(byte[] Content, string FileName)?> ExportOrderInfoAsync(Guid orderId)
    {
        var order = await _orders.GetByIdAsync(orderId);
        if (order is null)
            return null;

        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Order Info");

        var row = 1;
        AddSectionTitle(sheet, ref row, "Order");
        AddKeyValue(sheet, ref row, "Order Number", order.OrderNumber);
        AddKeyValue(sheet, ref row, "Source", order.Source);
        AddKeyValue(sheet, ref row, "Status", order.Status.ToString());
        AddKeyValue(sheet, ref row, "Sell Price (GEL)", order.SellPriceInGel);
        AddKeyValue(sheet, ref row, "Created By", order.CreatedByEmail);
        row++;

        AddSectionTitle(sheet, ref row, "Party");
        if (order.PersonParty is not null)
        {
            AddKeyValue(sheet, ref row, "Type", "Person");
            AddKeyValue(sheet, ref row, "First Name", order.PersonParty.FirstName);
            AddKeyValue(sheet, ref row, "Last Name", order.PersonParty.LastName);
            AddKeyValue(sheet, ref row, "Birth Date", FormatDate(order.PersonParty.BirthDate));
            AddKeyValue(sheet, ref row, "Email", order.PersonParty.Email);
            AddKeyValue(sheet, ref row, "Phone", order.PersonParty.Phone);
        }
        else if (order.CompanyParty is not null)
        {
            AddKeyValue(sheet, ref row, "Type", "Company");
            AddKeyValue(sheet, ref row, "Company Name", order.CompanyParty.CompanyName);
            AddKeyValue(sheet, ref row, "Registration Number", order.CompanyParty.RegistrationNumber);
            AddKeyValue(sheet, ref row, "Contact Person", order.CompanyParty.ContactPerson);
            AddKeyValue(sheet, ref row, "Email", order.CompanyParty.Email);
            AddKeyValue(sheet, ref row, "Phone", order.CompanyParty.Phone);
        }
        row++;

        AddSectionTitle(sheet, ref row, "Tour");
        AddKeyValue(sheet, ref row, "Name", order.Tour.Name);
        AddKeyValue(sheet, ref row, "Start Date", order.Tour.StartDate.ToString("yyyy-MM-dd"));
        AddKeyValue(sheet, ref row, "End Date", order.Tour.EndDate.ToString("yyyy-MM-dd"));
        AddKeyValue(sheet, ref row, "Passenger Count", order.Tour.PassengerCount);
        AddKeyValue(sheet, ref row, "Supplier", order.Tour.Supplier?.Name);
        row++;

        AddSectionTitle(sheet, ref row, "Passengers");
        AddTableHeader(sheet, ref row, "First Name", "Last Name", "Birth Date", "Document Number");
        foreach (var passenger in order.Tour.Passengers)
        {
            sheet.Cell(row, 1).Value = passenger.FirstName;
            sheet.Cell(row, 2).Value = passenger.LastName;
            sheet.Cell(row, 3).Value = FormatDate(passenger.BirthDate);
            sheet.Cell(row, 4).Value = passenger.DocumentNumber ?? string.Empty;
            row++;
        }
        row++;

        AddSectionTitle(sheet, ref row, "Air Tickets");
        AddTableHeader(sheet, ref row, "From", "To", "Flight Date", "Company", "Quantity", "Price");
        foreach (var ticket in order.Tour.AirTickets)
        {
            sheet.Cell(row, 1).Value = $"{ticket.CountryFrom}/{ticket.CityFrom}";
            sheet.Cell(row, 2).Value = $"{ticket.CountryTo}/{ticket.CityTo}";
            sheet.Cell(row, 3).Value = ticket.FlightDate.ToString("yyyy-MM-dd");
            sheet.Cell(row, 4).Value = ticket.FlightCompanyName;
            sheet.Cell(row, 5).Value = ticket.Quantity;
            sheet.Cell(row, 6).Value = FormatPrice(ticket.Price.Amount, ticket.Price.Currency.ToString());
            row++;
        }
        row++;

        AddSectionTitle(sheet, ref row, "Hotel Bookings");
        AddTableHeader(sheet, ref row, "Hotel", "Check-in", "Check-out", "Price");
        foreach (var hotel in order.Tour.HotelBookings)
        {
            sheet.Cell(row, 1).Value = hotel.HotelName;
            sheet.Cell(row, 2).Value = hotel.CheckIn.ToString("yyyy-MM-dd");
            sheet.Cell(row, 3).Value = hotel.CheckOut.ToString("yyyy-MM-dd");
            sheet.Cell(row, 4).Value = FormatPrice(hotel.Price.Amount, hotel.Price.Currency.ToString());
            row++;
        }
        row++;

        AddSectionTitle(sheet, ref row, "Extra Services");
        AddTableHeader(sheet, ref row, "Description", "Price");
        foreach (var extra in order.Tour.ExtraServices)
        {
            sheet.Cell(row, 1).Value = extra.Description;
            sheet.Cell(row, 2).Value = FormatPrice(extra.Price.Amount, extra.Price.Currency.ToString());
            row++;
        }
        row++;

        AddSectionTitle(sheet, ref row, "Payments");
        AddTableHeader(sheet, ref row, "Bank", "Paid Date", "Amount");
        foreach (var payment in order.Payments)
        {
            sheet.Cell(row, 1).Value = payment.BankName ?? string.Empty;
            sheet.Cell(row, 2).Value = payment.PaidDate.ToString("yyyy-MM-dd");
            sheet.Cell(row, 3).Value = FormatPrice(payment.Price.Amount, payment.Price.Currency.ToString());
            row++;
        }

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var safeOrderNumber = string.Concat(order.OrderNumber
            .Where(ch => !Path.GetInvalidFileNameChars().Contains(ch)));
        var fileName = $"order-info-{safeOrderNumber}.xlsx";

        return (stream.ToArray(), fileName);
    }

    private static void AddSectionTitle(IXLWorksheet sheet, ref int row, string title)
    {
        sheet.Cell(row, 1).Value = title;
        sheet.Cell(row, 1).Style.Font.Bold = true;
        row++;
    }

    private static void AddKeyValue(IXLWorksheet sheet, ref int row, string key, object? value)
    {
        sheet.Cell(row, 1).Value = key;
        sheet.Cell(row, 1).Style.Font.Bold = true;
        sheet.Cell(row, 2).Value = value?.ToString() ?? string.Empty;
        row++;
    }

    private static void AddTableHeader(IXLWorksheet sheet, ref int row, params string[] headers)
    {
        for (var column = 0; column < headers.Length; column++)
        {
            var cell = sheet.Cell(row, column + 1);
            cell.Value = headers[column];
            cell.Style.Font.Bold = true;
        }

        row++;
    }

    private static string FormatDate(DateOnly? date)
    {
        return date?.ToString("yyyy-MM-dd") ?? string.Empty;
    }

    private static string FormatPrice(decimal amount, string currency)
    {
        return $"{amount:0.##} {currency}";
    }
}
