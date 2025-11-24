using BusinessReportsManager.Application.DTOs.PriceCurrency;

namespace BusinessReportsManager.Application.DTOs.ExtraService;

public class ExtraServiceDto
{
    public string Description { get; set; } = string.Empty;
    public PriceCurrencyDto Price { get; set; } = new();
}