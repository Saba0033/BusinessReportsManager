namespace BusinessReportsManager.Domain.Interfaces;

public interface IOrderNumberGenerator
{
    Task<string> NextOrderNumberAsync(CancellationToken ct = default);
}