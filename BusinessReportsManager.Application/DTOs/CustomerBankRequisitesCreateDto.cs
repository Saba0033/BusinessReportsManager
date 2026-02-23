namespace BusinessReportsManager.Application.DTOs;

public class CustomerBankRequisitesCreateDto
{
    public string BankName { get; set; } = string.Empty;
    public string? AccountHolderFullName { get; set; }
    public string? Iban { get; set; }
    public string? AccountNumber { get; set; }
    public string? Swift { get; set; }
    public string? Comment { get; set; }
}
