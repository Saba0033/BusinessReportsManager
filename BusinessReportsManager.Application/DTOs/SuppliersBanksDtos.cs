namespace BusinessReportsManager.Application.DTOs;

public record SupplierDto(Guid Id, string Name, string? TaxId, string? ContactEmail, string? Phone);
public record CreateSupplierDto(string Name, string? TaxId, string? ContactEmail, string? Phone);

public record BankDto(Guid Id, string Name, string? Swift, string? AccountNumber);
public record CreateBankDto(string Name, string? Swift, string? AccountNumber);