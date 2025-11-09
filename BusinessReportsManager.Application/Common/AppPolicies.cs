namespace BusinessReportsManager.Application.Common;

public static class AppPolicies
{
    public const string CanViewAllOrders = nameof(CanViewAllOrders);
    public const string CanEditAllOrders = nameof(CanEditAllOrders);
    public const string CanEditOwnOpenOrders = nameof(CanEditOwnOpenOrders);
}