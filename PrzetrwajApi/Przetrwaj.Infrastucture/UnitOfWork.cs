using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Infrastucture.Context;

namespace Przetrwaj.Infrastucture;

internal class UnitOfWork : IUnitOfWork
{
	private readonly ApplicationDbContext _appDbContext;
	//private readonly TheOtherDbContext _userDbContext;

	public UnitOfWork(ApplicationDbContext dbContext)
	{
		_appDbContext = dbContext;
	}
	public async Task SaveChangesAsync(CancellationToken cancellationToken)
	{
		//UpdateEntitiesTime();
		await _appDbContext.SaveChangesAsync(cancellationToken);
	}

	//private void UpdateEntitiesTime()
	//{
	//	var ent = _dbContext.ChangeTracker.Entries<Entity>();
	//	foreach (var entity in ent)
	//	{
	//		if (entity.State == EntityState.Added)
	//			entity.Entity.CreatedAt = entity.Entity.UpdatedAt = DateTime.Now;
	//		else if (entity.State == EntityState.Modified)
	//			entity.Entity.UpdatedAt = DateTime.Now;
	//	}
	//}
}