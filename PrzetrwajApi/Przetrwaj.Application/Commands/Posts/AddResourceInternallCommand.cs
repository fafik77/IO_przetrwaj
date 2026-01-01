using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Application.Commands.Posts;

public class AddResourceInternallCommand : AddPostCommand, ICommand<PostCompleteDataDto>
{
	public CategoryType Category { get; set; } = CategoryType.Resource;
	public required string IdAutor { get; set; }
}
