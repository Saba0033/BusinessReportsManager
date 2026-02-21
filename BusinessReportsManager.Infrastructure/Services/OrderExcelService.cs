using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs.Order;
using ClosedXML.Excel;

namespace BusinessReportsManager.Infrastructure.Services;

public class OrderExcelService : IOrderExcelService
{
    private readonly IOrderService _orderService;

    public OrderExcelService(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<byte[]> GenerateOrderExcelAsync(Guid orderId)
    {
        var order = await _orderService.GetByIdAsync(orderId);
        if (order is null)
            throw new KeyNotFoundException($"Order {orderId} not found.");

        using var workbook = new XLWorkbook();

        AddOrderInfoSheet(workbook, order);
        AddPassengersSheet(workbook, order);
        AddAirTicketsSheet(workbook, order);
        AddHotelBookingsSheet(workbook, order);
        AddExtraServicesSheet(workbook, order);
        AddPaymentsSheet(workbook, order);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static void AddOrderInfoSheet(XLWorkbook workbook, OrderDto order)
    {
        var ws = workbook.Worksheets.Add("Order");

        var labels = new (string Label, string Value)[]
        {
            ("Order Number", order.OrderNumber),
            ("Status", order.Status.ToString()),
            ("Source", order.Source),
            ("Sell Price (GEL)", order.SellPriceInGel.ToString("N2")),
            ("Created By", order.CreatedByEmail ?? ""),
            ("Party Name", order.Party.FullName),
            ("Party Email", order.Party.Email),
            ("Party Phone", order.Party.Phone ?? ""),
            ("Tour Name", order.Tour.Name),
            ("Tour Start", order.Tour.StartDate.ToString("yyyy-MM-dd")),
            ("Tour End", order.Tour.EndDate.ToString("yyyy-MM-dd")),
            ("Passenger Count", order.Tour.PassengerCount.ToString()),
            ("Supplier", order.Tour.Supplier.Name),
        };

        for (var i = 0; i < labels.Length; i++)
        {
            ws.Cell(i + 1, 1).Value = labels[i].Label;
            ws.Cell(i + 1, 2).Value = labels[i].Value;
            ws.Cell(i + 1, 1).Style.Font.Bold = true;
        }

        ws.Columns().AdjustToContents();
    }

    private static void AddPassengersSheet(XLWorkbook workbook, OrderDto order)
    {
        if (order.Tour.Passengers.Count == 0) return;

        var ws = workbook.Worksheets.Add("Passengers");
        ws.Cell(1, 1).Value = "#";
        ws.Cell(1, 2).Value = "Full Name";
        StyleHeader(ws, 2);

        for (var i = 0; i < order.Tour.Passengers.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = i + 1;
            ws.Cell(i + 2, 2).Value = order.Tour.Passengers[i].FullName;
        }

        ws.Columns().AdjustToContents();
    }

    private static void AddAirTicketsSheet(XLWorkbook workbook, OrderDto order)
    {
        if (order.Tour.AirTickets.Count == 0) return;

        var ws = workbook.Worksheets.Add("Air Tickets");
        string[] headers = ["From Country", "To Country", "From City", "To City", "Flight Date", "Amount", "Currency", "Rate to GEL", "Price in GEL"];
        for (var c = 0; c < headers.Length; c++)
            ws.Cell(1, c + 1).Value = headers[c];
        StyleHeader(ws, headers.Length);

        for (var i = 0; i < order.Tour.AirTickets.Count; i++)
        {
            var t = order.Tour.AirTickets[i];
            var row = i + 2;
            ws.Cell(row, 1).Value = t.CountryFrom;
            ws.Cell(row, 2).Value = t.CountryTo;
            ws.Cell(row, 3).Value = t.CityFrom;
            ws.Cell(row, 4).Value = t.CityTo;
            ws.Cell(row, 5).Value = t.FlightDate.ToString("yyyy-MM-dd");
            ws.Cell(row, 6).Value = t.Price.Amount;
            ws.Cell(row, 7).Value = t.Price.Currency.ToString();
            ws.Cell(row, 8).Value = t.Price.ExchangeRateToGel ?? 0;
            ws.Cell(row, 9).Value = t.Price.PriceInGel ?? 0;
        }

        ws.Columns().AdjustToContents();
    }

    private static void AddHotelBookingsSheet(XLWorkbook workbook, OrderDto order)
    {
        if (order.Tour.HotelBookings.Count == 0) return;

        var ws = workbook.Worksheets.Add("Hotel Bookings");
        string[] headers = ["#", "Amount", "Currency", "Rate to GEL", "Price in GEL"];
        for (var c = 0; c < headers.Length; c++)
            ws.Cell(1, c + 1).Value = headers[c];
        StyleHeader(ws, headers.Length);

        for (var i = 0; i < order.Tour.HotelBookings.Count; i++)
        {
            var h = order.Tour.HotelBookings[i];
            var row = i + 2;
            ws.Cell(row, 1).Value = i + 1;
            ws.Cell(row, 2).Value = h.Price.Amount;
            ws.Cell(row, 3).Value = h.Price.Currency.ToString();
            ws.Cell(row, 4).Value = h.Price.ExchangeRateToGel ?? 0;
            ws.Cell(row, 5).Value = h.Price.PriceInGel ?? 0;
        }

        ws.Columns().AdjustToContents();
    }

    private static void AddExtraServicesSheet(XLWorkbook workbook, OrderDto order)
    {
        if (order.Tour.ExtraServices.Count == 0) return;

        var ws = workbook.Worksheets.Add("Extra Services");
        string[] headers = ["Description", "Amount", "Currency", "Rate to GEL", "Price in GEL"];
        for (var c = 0; c < headers.Length; c++)
            ws.Cell(1, c + 1).Value = headers[c];
        StyleHeader(ws, headers.Length);

        for (var i = 0; i < order.Tour.ExtraServices.Count; i++)
        {
            var e = order.Tour.ExtraServices[i];
            var row = i + 2;
            ws.Cell(row, 1).Value = e.Description;
            ws.Cell(row, 2).Value = e.Price.Amount;
            ws.Cell(row, 3).Value = e.Price.Currency.ToString();
            ws.Cell(row, 4).Value = e.Price.ExchangeRateToGel ?? 0;
            ws.Cell(row, 5).Value = e.Price.PriceInGel ?? 0;
        }

        ws.Columns().AdjustToContents();
    }

    private static void AddPaymentsSheet(XLWorkbook workbook, OrderDto order)
    {
        if (order.Payments.Count == 0) return;

        var ws = workbook.Worksheets.Add("Payments");
        string[] headers = ["Bank Name", "Paid Date", "Amount", "Currency", "Rate to GEL", "Price in GEL"];
        for (var c = 0; c < headers.Length; c++)
            ws.Cell(1, c + 1).Value = headers[c];
        StyleHeader(ws, headers.Length);

        for (var i = 0; i < order.Payments.Count; i++)
        {
            var p = order.Payments[i];
            var row = i + 2;
            ws.Cell(row, 1).Value = p.BankName ?? "";
            ws.Cell(row, 2).Value = p.PaidDate.ToString("yyyy-MM-dd");
            ws.Cell(row, 3).Value = p.Price.Amount;
            ws.Cell(row, 4).Value = p.Price.Currency.ToString();
            ws.Cell(row, 5).Value = p.Price.ExchangeRateToGel ?? 0;
            ws.Cell(row, 6).Value = p.Price.PriceInGel ?? 0;
        }

        ws.Columns().AdjustToContents();
    }

    private static void StyleHeader(IXLWorksheet ws, int columnCount)
    {
        var headerRange = ws.Range(1, 1, 1, columnCount);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
    }
}
