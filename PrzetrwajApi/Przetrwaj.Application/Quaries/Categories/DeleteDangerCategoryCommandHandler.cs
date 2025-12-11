using Przetrwaj.Application.Commands.Categories;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;

internal class DeleteDangerCategoryCommandHandler
    : ICommandHandler<DeleteDangerCategoryCommand, bool>
{
    private readonly ICategoryRepository _repo;
    private readonly IUnitOfWork _uow;

    public DeleteDangerCategoryCommandHandler(ICategoryRepository repo, IUnitOfWork uow)
    {
        _repo = repo; _uow = uow;
    }

    public async Task<bool> Handle(DeleteDangerCategoryCommand cmd, CancellationToken ct)
    {
        var ok = await _repo.DeleteDangerByIdAsync(cmd.IdCategory, ct);
        if (!ok) return false;
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}