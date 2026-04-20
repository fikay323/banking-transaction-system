namespace practice.Application.Interfaces;

public interface IIdempotencyRepository
{
    Task<bool> EnsureIdempotencyAsync(Guid key, string requestName);
}
