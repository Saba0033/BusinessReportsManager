using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Application.DTOs;

public record CreateOrderDto(Guid OrderPartyId, Guid TourId, string Source, MoneyDto SellPrice, MoneyDto TicketSelfCost);
public record UpdateOrderDto(string? Source, MoneyDto SellPrice, MoneyDto TicketSelfCost, string RowVersionBase64);
public record OrderListItemDto(Guid Id, string OrderNumber, string Party, string Tour, MoneyDto SellPrice, OrderStatus Status, string OwnedByUserEmail, string PaymentStatus);
public record OrderDetailsDto(Guid Id, string OrderNumber, OrderStatus Status, string Source, string OwnedByUserEmail, MoneyDto SellPrice, MoneyDto TicketSelfCost, string PaymentStatus,
    List<PassengerDto> Passengers, List<PaymentDto> Payments);

public record PassengerDto(Guid Id, string FirstName, string LastName, bool IsPrimary, DateOnly? BirthDate, string? DocumentNumber);
public record CreatePassengerDto(string FirstName, string LastName, bool IsPrimary, DateOnly? BirthDate, string? DocumentNumber);

public record PaymentDto(Guid Id, MoneyDto Amount, Guid BankId, string BankName, DateOnly PaidDate, string? Reference);
public record CreatePaymentDto(MoneyDto Amount, Guid BankId, DateOnly PaidDate, string? Reference);