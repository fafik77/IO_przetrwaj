using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Posts;

public record AddPostCommand
{
	[Required]
	[MaxLength(200)]
	[MinLength(3)]
	public required string Title { get; set; }
	[MaxLength(2000)]
	public string? Description { get; set; }
	[Required]
	public int IdCategory { get; set; }
	[MaxLength(100)]
	public string? CustomCategory { get; set; }
	[Required]
	public required LatLong LatLong { get; set; }
	public RegionPrecision RegionPrecision { get; set; } = RegionPrecision.GMI;
}

