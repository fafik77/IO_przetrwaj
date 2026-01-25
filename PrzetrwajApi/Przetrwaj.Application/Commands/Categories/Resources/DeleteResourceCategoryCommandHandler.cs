using Przetrwaj.Application.Commands.Categories.Resources;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions;

internal class DeleteResourceCategoryCommandHandler
	: ICommandHandler<DeleteResourceCategoryCommand, bool>
{
	private readonly ICategoryRepository _repo;
	private readonly IUnitOfWork _uow;

	public DeleteResourceCategoryCommandHandler(ICategoryRepository repo, IUnitOfWork uow)
	{
		_repo = repo; _uow = uow;
	}

	public async Task<bool> Handle(DeleteResourceCategoryCommand cmd, CancellationToken ct)
	{
		var cat = await _repo.GetResourceByIdAsync(cmd.IdCategory, ct);
		if (cat is null) return false;
		_repo.Delete(cat);
		try
		{
			await _uow.SaveChangesAsync(ct);
		}
		catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
		{
			throw new BadUpdateCommand(ex.InnerException.Message);
		}
		return true;
	}
}