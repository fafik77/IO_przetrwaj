using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions.RegionException;
using Przetrwaj.Infrastucture.Context;

namespace Przetrwaj.Infrastucture.Repositories;

public class RegionRepository : IRegionRepository
{
	private readonly ApplicationDbContext _dbContext;

	public RegionRepository(ApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task AddAsync(Region region, CancellationToken cancellationToken)
	{
		await _dbContext.Regions.AddAsync(region, cancellationToken);
	}

	public async Task<Region?> GetByIdAsync(int id, CancellationToken cancellationToken)
	{
		var region = await _dbContext.Regions.FirstOrDefaultAsync(r => r.IdRegion == id, cancellationToken);
		//if (region == null) throw new RegionNotFoundException(id);
		return region;
	}

	public async Task<IEnumerable<Region>> GetAllAsync(CancellationToken cancellationToken)
	{
		var regions = await _dbContext.Regions.AsNoTracking().ToListAsync(cancellationToken);
		return regions;
	}

	public void Update(Region item)
	{
		_dbContext.Regions.Update(item);
	}

	public void Delete(Region item)
	{
		_dbContext.Regions.Remove(item);
	}
}
