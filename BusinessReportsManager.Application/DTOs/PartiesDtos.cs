namespace BusinessReportsManager.Application.DTOs;

public abstract record OrderPartyDto(Guid Id, string Email, string? Phone);

public record PersonPartyDto(Guid Id, string Email, string? Phone, string FirstName, string LastName, DateOnly? BirthDate)
    : OrderPartyDto(Id, Email, Phone);

public record CompanyPartyDto(Guid Id, string Email, string? Phone, string CompanyName, string? RegistrationNumber, string? ContactPerson)
    : OrderPartyDto(Id, Email, Phone);

public record CreatePersonPartyDto(string Email, string? Phone, string FirstName, string LastName, DateOnly? BirthDate);
public record CreateCompanyPartyDto(string Email, string? Phone, string CompanyName, string? RegistrationNumber, string? ContactPerson);