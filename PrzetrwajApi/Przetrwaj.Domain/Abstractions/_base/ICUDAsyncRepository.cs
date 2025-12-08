namespace Przetrwaj.Domain.Abstractions._base;

public interface ICUDAsyncRepository<T>
{
	Task AddAsync(T item, CancellationToken cancellationToken = default);
	void Update(T item);
	void Delete(T item);
}
