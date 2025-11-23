// using BusinessReportsManager.Domain.Enums;
//
// namespace BusinessReportsManager.Domain.ValueObjects;
//
// public record class Money
// {
//     public decimal Amount { get; init; }
//     public Currency Currency { get; init; }
//
//     public Money() { }
//     public Money(decimal amount, Currency currency)
//     {
//         Amount = amount;
//         Currency = currency;
//     }
//     public static Money Zero(Currency currency) => new(0m, currency);
//     public override string ToString() => $"{Amount:0.00} {Currency}";
// }