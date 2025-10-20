namespace BusinessReportsManager.Domain.Common;

public enum Currency
{
    GEL = 1,
    EUR = 2,
    USD = 3
}

public record Money(decimal Amount, Currency Currency);