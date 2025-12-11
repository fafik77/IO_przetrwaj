using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Application.Commands.Categories;

internal class CreateResourceCategoryCommandHandler
    : ICommandHandler<CreateResourceCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateResourceCategoryCommandHandler(ICategoryRepository repo, IUnitOfWork uow)
    {
        _repo = repo; _uow = uow;
    }

    public async Task<CategoryDto> Handle(CreateResourceCategoryCommand request, CancellationToken ct)
    {
		var cat = new CategoryResource { Name= request.Name };
        var e = await _repo.AddAsync(cat, ct);
        await _uow.SaveChangesAsync(ct);
        return (CategoryDto)e;
    }
}
