// using BusinessReportsManager.Domain.Entities;
// using BusinessReportsManager.Domain.Enums;
//
// namespace BusinessReportsManager.Domain.Queries;
//
// public static class OrderQueries
// {
//     public static IQueryable<Order> Open(this IQueryable<Order> source) => source.Where(o => o.Status == OrderStatus.Open);
//     public static IQueryable<Order> OwnedBy(this IQueryable<Order> source, string userId) => source.Where(o => o.OwnedByUserId == userId);
// }