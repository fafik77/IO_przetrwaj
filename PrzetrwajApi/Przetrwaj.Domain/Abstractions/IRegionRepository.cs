using Przetrwaj.Domain.Abstractions._base;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models;

namespace Przetrwaj.Domain.Abstractions;

public interface IRegionRepository : ICUDAsyncRepository<IRegionInfo>
{
	public Task<AllRegions> GetAllAsync(CancellationToken cancellationToken = default);
	public Task<IRegionInfo?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
