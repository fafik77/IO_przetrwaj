using Przetrwaj.Application.Configuration.Commands;

namespace Przetrwaj.Application.Commands.Categories;

public class DeleteDangerCategoryCommand : ICommand<bool>
{
    public int IdCategory { get; set; }
}
