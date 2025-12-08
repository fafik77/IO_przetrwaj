namespace Przetrwaj.Domain.Abstractions._base;

public interface IGetsAsyncRepository<T>
{
	public Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
	public Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
