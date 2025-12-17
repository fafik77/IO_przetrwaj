using Microsoft.AspNetCore.Http;
using Przetrwaj.Application.Commands.Posts.Attachments;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Posts;

public class AddDangerCommand
{
	[Required]
	[MaxLength(200)]
	[MinLength(3)]
	public required string Title { get; set; }
	[MaxLength(2000)]
	public string? Description { get; set; }
	public int IdCategory { get; set; }
	[MaxLength(100)]
	public string? CustomCategory { get; set; }
	public int IdRegion { get; set; }

	public IList<string>? AlternateDescriptions { get; set; }
	public IFormFileCollection? Files { get; set; }
}

