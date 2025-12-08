using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Application.Commands.Categories;

public class CreateCategoryCommand : ICommand<CategoryDto>
{
	public required string Name { get; set; }
	public required CategoryType Type { get; set; }
}
