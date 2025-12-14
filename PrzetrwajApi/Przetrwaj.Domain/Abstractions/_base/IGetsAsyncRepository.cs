namespace Przetrwaj.Domain.Abstractions._base;

public interface IGetsAsyncRepository<T> : IGetsAsyncRepository<T, int>
{
}
/// <summary>
/// Declares: GetAllAsync, GetByIdAsync
/// </summary>
/// <typeparam name="T">Type of object to return</typeparam>
/// <typeparam name="TId">id type</typeparam>
public interface IGetsAsyncRepository<T, TId>
{
	public Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
	public Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
}
