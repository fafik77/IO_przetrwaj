using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Entities;
using System.Security.Claims;

namespace Przetrwaj.Application.Commands.Posts;

public record AddDangerInternallCommand : ICommand<(Post, AddAttachmentsResult?)>
{
	public required AddPostCommand AddPostCommand { get; set; }
	public CategoryType Category { get; set; } = CategoryType.Danger;
	public required string IdAutor { get; set; }
	public required ClaimsPrincipal ClaimsPrincipal { get; set; }
}

