using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Application.DTOs.AirTicket;
using BusinessReportsManager.Application.DTOs.HotelBooking;
using BusinessReportsManager.Application.DTOs.Order;
using BusinessReportsManager.Application.DTOs.OrderParty;
using BusinessReportsManager.Application.DTOs.Passenger;
using BusinessReportsManager.Application.DTOs.Payment;
using BusinessReportsManager.Application.DTOs.PriceCurrency;
using BusinessReportsManager.Application.DTOs.Supplier;
using BusinessReportsManager.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace BusinessReportsManager.Api.SwaggerExamples;

public sealed class LoginRequestExample : IExamplesProvider<LoginRequest>
{
    public LoginRequest GetExamples() => new("supervisor", "P@ssword1");
}

public sealed class RegisterRequestExample : IExamplesProvider<RegisterRequest>
{
    public RegisterRequest GetExamples() => new(
        "newemployee",
        "newemployee@example.com",
        "P@ssword1",
        "Employee");
}

public sealed class PaymentCreateDtoExample : IExamplesProvider<PaymentCreateDto>
{
    public PaymentCreateDto GetExamples() => new()
    {
        BankName = "Bank of Georgia",
        PaidDate = new DateOnly(2025, 8, 1),
        Price = new PriceCurrencyCreateDto
        {
            Currency = Currency.GEL,
            Amount = 1500.00m,
            ExchangeRateToGel = 1m
        }
    };
}

public sealed class AccountingCommentUpdateDtoExample : IExamplesProvider<AccountingCommentUpdateDto>
{
    public AccountingCommentUpdateDto GetExamples() => new()
    {
        Comment = "Invoice verified; awaiting transfer."
    };
}

internal static class OrderCreatePayloadSample
{
    public static OrderCreateDto Build()
    {
        var start = new DateOnly(2025, 7, 10);
        var end = new DateOnly(2025, 7, 17);
        return new OrderCreateDto
        {
            Party = new PartyCreateDto
            {
                FullName = "Nino Kapanadze",
                Email = "nino.customer@example.com",
                Phone = "+995555123456",
                PersonalNumber = "61001012345"
            },
            Destination = "Italy — Rome & Florence",
            StartDate = start,
            EndDate = end,
            PassengerCount = 2,
            Supplier = new SupplierCreateDto { Name = "EuroTravel Georgia" },
            AirTickets =
            [
                new AirTicketCreateDto
                {
                    CountryTo = "Italy",
                    CityTo = "Rome",
                    FlightDate = start,
                    Price = new PriceCurrencyCreateDto
                    {
                        Currency = Currency.USD,
                        Amount = 400m,
                        ExchangeRateToGel = 2.75m
                    }
                }
            ],
            HotelBookings =
            [
                new HotelBookingCreateDto
                {
                    Price = new PriceCurrencyCreateDto
                    {
                        Currency = Currency.EUR,
                        Amount = 600m,
                        ExchangeRateToGel = 3.0m
                    }
                }
            ],
            Source = "API",
            TourType = "Leisure",
            SellPriceInGel = 5000m,
            TotalExpenseInGel = 3200m,
            CustomerBankRequisites = new CustomerBankRequisitesCreateDto
            {
                BankName = "Bank of Georgia",
                AccountHolderFullName = "Nino Kapanadze",
                Iban = "GE00BG0000000000000000",
                Swift = "BAGAGE22",
                Comment = "Optional note"
            },
            Passengers =
            [
                new PassengerCreateDto { FullName = "Alice Smith" },
                new PassengerCreateDto { FullName = "Bob Smith" }
            ],
            TicketNet = 1100m,
            TicketSupplier = "Air Agent Tbilisi",
            HotelNet = 1800m,
            HotelSupplier = "Hotel Partner",
            TransferNet = 150m,
            TransferSupplier = "Transfer Co",
            InsuranceNet = 100m,
            InsuranceSupplier = "Insure.ge",
            OtherServiceNet = 50m,
            OtherServiceSupplier = "Other"
        };
    }
}

public sealed class OrderCreateDtoExample : IExamplesProvider<OrderCreateDto>
{
    public OrderCreateDto GetExamples() => OrderCreatePayloadSample.Build();
}

public sealed class OrderEditDtoExample : IExamplesProvider<OrderEditDto>
{
    public OrderEditDto GetExamples()
    {
        var o = OrderCreatePayloadSample.Build();
        return new OrderEditDto
        {
            Party = o.Party,
            Destination = o.Destination,
            StartDate = o.StartDate,
            EndDate = o.EndDate,
            PassengerCount = o.PassengerCount,
            Supplier = o.Supplier,
            AirTickets = o.AirTickets,
            HotelBookings = o.HotelBookings,
            Source = o.Source,
            TourType = o.TourType,
            ManagerName = o.ManagerName,
            SellPriceInGel = o.SellPriceInGel,
            CustomerBankRequisites = o.CustomerBankRequisites,
            TotalExpenseInGel = o.TotalExpenseInGel,
            Passengers = o.Passengers,
            TicketNet = o.TicketNet,
            TicketSupplier = o.TicketSupplier,
            HotelNet = o.HotelNet,
            HotelSupplier = o.HotelSupplier,
            TransferNet = o.TransferNet,
            TransferSupplier = o.TransferSupplier,
            InsuranceNet = o.InsuranceNet,
            InsuranceSupplier = o.InsuranceSupplier,
            OtherServiceNet = o.OtherServiceNet,
            OtherServiceSupplier = o.OtherServiceSupplier
        };
    }
}
