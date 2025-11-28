using FluentValidation;
using BusinessReportsManager.Application.DTOs;

namespace BusinessReportsManager.Application.Validation;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MinimumLength(2);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}
public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateDtoValidator()
    {
        RuleFor(x => x.Party).NotNull().SetValidator(new PartyCreateDtoValidator());
        RuleFor(x => x.Tour).NotNull().SetValidator(new TourCreateDtoValidator());

        RuleFor(x => x.CreatedById).NotEmpty();
        RuleFor(x => x.CreatedByEmail).NotEmpty().EmailAddress();

        RuleFor(x => x.Source).NotEmpty();

        RuleFor(x => x.SellPriceInGel)
            .GreaterThanOrEqualTo(0);

        RuleForEach(x => x.Passengers).SetValidator(new PassengerCreateDtoValidator());
        RuleForEach(x => x.Payments).SetValidator(new PaymentCreateDtoValidator());
    }
}
public class OrderEditDtoValidator : AbstractValidator<OrderEditDto>
{
    public OrderEditDtoValidator()
    {
        RuleFor(x => x.SellPriceInGel)
            .GreaterThanOrEqualTo(0)
            .When(x => x.SellPriceInGel.HasValue);

        RuleFor(x => x.Source)
            .NotEmpty()
            .When(x => x.Source is not null);
    }
}
public class PartyCreateDtoValidator : AbstractValidator<PartyCreateDto>
{
    public PartyCreateDtoValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(x => x == "Person" || x == "Company")
            .WithMessage("Type must be 'Person' or 'Company'.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .When(x => x.Phone is not null);

      
        When(x => x.Type == "Person", () =>
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("FirstName is required for Person.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("LastName is required for Person.");

            RuleFor(x => x.CompanyName)
                .Empty()
                .WithMessage("CompanyName must be empty for Person.");

            RuleFor(x => x.RegistrationNumber)
                .Empty()
                .WithMessage("RegistrationNumber must be empty for Person.");

            RuleFor(x => x.ContactPerson)
                .Empty()
                .WithMessage("ContactPerson must be empty for Person.");
        });

        When(x => x.Type == "Company", () =>
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty()
                .WithMessage("CompanyName is required for Company.");

            RuleFor(x => x.RegistrationNumber)
                .NotEmpty()
                .WithMessage("RegistrationNumber is required for Company.");

            RuleFor(x => x.ContactPerson)
                .NotEmpty()
                .WithMessage("ContactPerson is required for Company.");

            RuleFor(x => x.FirstName)
                .Empty()
                .WithMessage("FirstName must be empty for Company.");

            RuleFor(x => x.LastName)
                .Empty()
                .WithMessage("LastName must be empty for Company.");

            RuleFor(x => x.BirthDate)
                .Null()
                .WithMessage("BirthDate must be empty for Company.");
        });
    }
}

public class TourCreateDtoValidator : AbstractValidator<TourCreateDto>
{
    public TourCreateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("EndDate cannot be earlier than StartDate.");

        RuleFor(x => x.PassengerCount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Supplier)
            .NotNull()
            .SetValidator(new SupplierCreateDtoValidator());

        RuleForEach(x => x.AirTickets)
            .SetValidator(new AirTicketCreateDtoValidator());

        RuleForEach(x => x.HotelBookings)
            .SetValidator(new HotelBookingCreateDtoValidator());

        RuleForEach(x => x.ExtraServices)
            .SetValidator(new ExtraServiceCreateDtoValidator());
    }
}
public class PassengerCreateDtoValidator : AbstractValidator<PassengerCreateDto>
{
    public PassengerCreateDtoValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();

        RuleFor(x => x.DocumentNumber)
            .MaximumLength(50)
            .When(x => x.DocumentNumber is not null);
    }
}
public class SupplierCreateDtoValidator : AbstractValidator<SupplierCreateDto>
{
    public SupplierCreateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();

        RuleFor(x => x.ContactEmail)
            .EmailAddress()
            .When(x => x.ContactEmail is not null);

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .When(x => x.Phone is not null);
    }
}
public class AirTicketCreateDtoValidator : AbstractValidator<AirTicketCreateDto>
{
    public AirTicketCreateDtoValidator()
    {
        RuleFor(x => x.CountryFrom).NotEmpty();
        RuleFor(x => x.CountryTo).NotEmpty();
        RuleFor(x => x.CityFrom).NotEmpty();
        RuleFor(x => x.CityTo).NotEmpty();

        RuleFor(x => x.FlightDate)
            .GreaterThan(DateOnly.MinValue);

        RuleFor(x => x.FlightCompanyName)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.Price)
            .NotNull()
            .SetValidator(new PriceCurrencyCreateDtoValidator());
    }
}
public class HotelBookingCreateDtoValidator : AbstractValidator<HotelBookingCreateDto>
{
    public HotelBookingCreateDtoValidator()
    {
        RuleFor(x => x.HotelName).NotEmpty();

        RuleFor(x => x.CheckOut)
            .GreaterThanOrEqualTo(x => x.CheckIn);

        RuleFor(x => x.Price)
            .NotNull()
            .SetValidator(new PriceCurrencyCreateDtoValidator());
    }
}
public class ExtraServiceCreateDtoValidator : AbstractValidator<ExtraServiceCreateDto>
{
    public ExtraServiceCreateDtoValidator()
    {
        RuleFor(x => x.Description).NotEmpty();

        RuleFor(x => x.Price)
            .NotNull()
            .SetValidator(new PriceCurrencyCreateDtoValidator());
    }
}
public class PriceCurrencyCreateDtoValidator : AbstractValidator<PriceCurrencyCreateDto>
{
    public PriceCurrencyCreateDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Currency)
            .IsInEnum();

        RuleFor(x => x.ExchangeRateToGel)
            .GreaterThan(0)
            .When(x => x.ExchangeRateToGel.HasValue);
    }
}
public class PaymentCreateDtoValidator : AbstractValidator<PaymentCreateDto>
{
    public PaymentCreateDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.Currency)
            .IsInEnum();

        RuleFor(x => x.BankName)
            .MaximumLength(100)
            .When(x => x.BankName is not null);

        RuleFor(x => x.Reference)
            .MaximumLength(100)
            .When(x => x.Reference is not null);
    }
}






