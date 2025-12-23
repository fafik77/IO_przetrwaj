using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Application.Commands.Posts;

public class AddDangerInternallCommand : AddDangerCommand , ICommand<PostCompleteDataDto>
{
	public CategoryType Category { get; set; } = CategoryType.Danger;
	public required string IdAutor { get; set; }
}

