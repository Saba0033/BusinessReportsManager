using FluentValidation;
using BusinessReportsManager.Application.DTOs;

namespace BusinessReportsManager.Application.Validation;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.OrderPartyId).NotEmpty();
        RuleFor(x => x.TourId).NotEmpty();
        RuleFor(x => x.Source).NotEmpty();
        RuleFor(x => x.SellPrice.Amount).GreaterThan(0);
        RuleFor(x => x.TicketSelfCost.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SellPrice.Currency).IsInEnum();
        RuleFor(x => x.TicketSelfCost.Currency).IsInEnum();
    }
}

public class CreateTourDtoValidator : AbstractValidator<CreateTourDto>
{
    public CreateTourDtoValidator()
    {
        RuleFor(x => x.StartDate).LessThanOrEqualTo(x => x.EndDate);
        RuleFor(x => x.PassengerCount).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Destinations).NotEmpty();
    }
}

public class CreatePassengerDtoValidator : AbstractValidator<CreatePassengerDto>
{
    public CreatePassengerDtoValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        When(x => x.IsPrimary, () =>
        {
            RuleFor(x => x.BirthDate).NotNull().WithMessage("BirthDate is required for primary passenger.");
        });
    }
}

public class CreatePaymentDtoValidator : AbstractValidator<CreatePaymentDto>
{
    public CreatePaymentDtoValidator()
    {
        RuleFor(x => x.Amount.Amount).GreaterThan(0);
        RuleFor(x => x.BankId).NotEmpty();
    }
}