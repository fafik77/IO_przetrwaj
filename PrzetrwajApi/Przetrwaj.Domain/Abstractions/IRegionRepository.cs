using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Domain.Abstractions;

public interface IRegionRepository
{
	public Task<AllRegions> GetAllAsync(CancellationToken ct = default);
	public Task<IRegionInfo?> GetByIdAsync(int id, CancellationToken ct = default);

	public Task AddAsync<T>(IEnumerable<T> regions, CancellationToken ct) where T : class, IRegionInfo;
	public Task AddAsync<T>(T region, CancellationToken ct) where T : class, IRegionInfo;
	public void Delete<T>(IEnumerable<T> regions) where T : class, IRegionInfo;
	public void Delete<T>(T region) where T : class, IRegionInfo;
	public void Update<T>(IEnumerable<T> regions) where T : class, IRegionInfo;
	public void Update<T>(T region) where T : class, IRegionInfo;
	public Task<IRegionInfo?> RegionFromLocationAsync(LatLong location, CancellationToken ct = default);
}
