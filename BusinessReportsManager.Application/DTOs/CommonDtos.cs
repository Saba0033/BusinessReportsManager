using BusinessReportsManager.Domain.Enums;

namespace BusinessReportsManager.Application.DTOs;

public record MoneyDto(decimal Amount, Currency Currency);

public record IdNameDto(Guid Id, string Name);