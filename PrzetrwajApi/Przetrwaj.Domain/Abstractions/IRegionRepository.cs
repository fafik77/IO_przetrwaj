using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models;

namespace Przetrwaj.Domain.Abstractions;

public interface IRegionRepository
{
	public Task<AllRegions> GetAllAsync(CancellationToken cancellationToken = default);
	public Task<IRegionInfo?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

	public Task AddAsync<T>(IEnumerable<T> regions, CancellationToken ct) where T : class, IRegionInfo;
	public Task AddAsync<T>(T region, CancellationToken ct) where T : class, IRegionInfo;
	public void Delete<T>(IEnumerable<T> regions) where T : class, IRegionInfo;
	public void Delete<T>(T region) where T : class, IRegionInfo;
	public void Update<T>(IEnumerable<T> regions) where T : class, IRegionInfo;
	public void Update<T>(T region) where T : class, IRegionInfo;
}
