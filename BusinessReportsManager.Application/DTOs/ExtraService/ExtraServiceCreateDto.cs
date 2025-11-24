using BusinessReportsManager.Application.DTOs.PriceCurrency;

namespace BusinessReportsManager.Application.DTOs.ExtraService;

public class ExtraServiceCreateDto
{
    public string Description { get; set; } = string.Empty;
    public PriceCurrencyCreateDto Price { get; set; } = new();
}