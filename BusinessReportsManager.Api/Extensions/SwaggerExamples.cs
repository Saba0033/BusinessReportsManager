using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace BusinessReportsManager.Api.Extensions;

public class CreateOrderExample : IExamplesProvider<CreateOrderDto>
{
    public CreateOrderDto GetExamples() =>
        new CreateOrderDto(
            OrderPartyId: Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            TourId: Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            Source: "Website",
            SellPrice: new MoneyDto(1200m, Currency.GEL),
            TicketSelfCost: new MoneyDto(800m, Currency.GEL));
}

public class CreateTourExample : IExamplesProvider<CreateTourDto>
{
    public CreateTourDto GetExamples() =>
        new CreateTourDto(
            Name: "Caucasus Getaway",
            StartDate: new DateOnly(2025, 5, 1),
            EndDate: new DateOnly(2025, 5, 7),
            PassengerCount: 2,
            Destinations: new List<DestinationDto> { new DestinationDto(Guid.Empty, "Georgia", "Tbilisi") });
}

public class CreatePassengerExample : IExamplesProvider<CreatePassengerDto>
{
    public CreatePassengerDto GetExamples() =>
        new CreatePassengerDto("Nino", "Beridze", true, new DateOnly(1990,1,1), "AB1234567");
}

public class CreatePaymentExample : IExamplesProvider<CreatePaymentDto>
{
    public CreatePaymentDto GetExamples() =>
        new CreatePaymentDto(new MoneyDto(300m, Currency.GEL), Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), new DateOnly(2025,1,15), "Advance");
}