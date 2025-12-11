using Przetrwaj.Application.Commands.Categories;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;

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
        var ok = await _repo.DeleteResourceByIdAsync(cmd.IdCategory, ct);
        if (!ok) return false;
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}