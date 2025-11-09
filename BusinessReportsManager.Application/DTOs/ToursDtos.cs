namespace BusinessReportsManager.Application.DTOs;

public record DestinationDto(Guid Id, string Country, string City);
public record AirTicketDto(Guid Id, string From, string To, DateOnly FlightDate, string? Pnr);
public record HotelBookingDto(Guid Id, string HotelName, DateOnly CheckIn, DateOnly CheckOut, string? ConfirmationNumber);
public record ExtraServiceDto(Guid Id, string Description);

public record TourDto(Guid Id, string Name, DateOnly StartDate, DateOnly EndDate, int PassengerCount,
    List<DestinationDto> Destinations,
    List<AirTicketDto> AirTickets,
    List<HotelBookingDto> HotelBookings,
    List<ExtraServiceDto> ExtraServices);

public record CreateTourDto(string Name, DateOnly StartDate, DateOnly EndDate, int PassengerCount, List<DestinationDto> Destinations);