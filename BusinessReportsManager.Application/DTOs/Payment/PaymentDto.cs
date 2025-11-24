using BusinessReportsManager.Application.DTOs.PriceCurrency;

namespace BusinessReportsManager.Application.DTOs.Payment;

public class PaymentDto
{
    public Guid Id { get; set; }

    public PriceCurrencyDto Price { get; set; } = new();

    public string? BankName { get; set; }
    public DateOnly PaidDate { get; set; }
}