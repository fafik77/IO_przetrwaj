using Przetrwaj.Domain.Abstractions._base;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Abstractions;

public interface IImpedimentsRepository : ICUDAsyncRepository<Impediment>
{
	public Task<IDictionary<short, string>> GetAllAsync(CancellationToken cancellationToken = default);
	public Task<Impediment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
