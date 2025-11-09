namespace BusinessReportsManager.Application.Common;

public record PagedRequest(int Page = 1, int PageSize = 20);

public class PagedResult<T>
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
}