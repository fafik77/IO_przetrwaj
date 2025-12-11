using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;

namespace Przetrwaj.Application.Commands.Categories;

public class CreateDangerCategoryCommand : ICommand<CategoryDto>
{
    public required string Name { get; set; }
}
