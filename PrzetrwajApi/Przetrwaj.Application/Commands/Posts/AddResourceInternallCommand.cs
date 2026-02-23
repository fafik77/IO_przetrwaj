using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos.Posts;
using System.Security.Claims;

namespace Przetrwaj.Application.Commands.Posts;

public class AddResourceInternallCommand : ICommand<PostCompleteDataDto>
{
	public required AddPostCommand AddPostCommand { get; set; }
	public CategoryType Category { get; set; } = CategoryType.Resource;
	public required string IdAutor { get; set; }
	public required IEnumerable<Claim> Claims { get; set; }
}
