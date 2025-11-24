using BusinessReportsManager.Domain.Enums;
using BusinessReportsManager.Application.DTOs.PriceCurrency;

namespace BusinessReportsManager.Application.DTOs.Payment;

public class PaymentCreateDto
{
    public PriceCurrencyCreateDto Price { get; set; } = new();

    public string? BankName { get; set; }
    public DateOnly PaidDate { get; set; }
}