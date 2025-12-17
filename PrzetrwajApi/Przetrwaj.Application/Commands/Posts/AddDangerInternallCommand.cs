using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos.Posts;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Application.Commands.Posts;

public class AddDangerInternallCommand : AddDangerCommand , ICommand<PostCompleteDataDto>
{
	public CategoryType Category { get; set; } = CategoryType.Danger;
	public required string IdAutor { get; set; }
}

