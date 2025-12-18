using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos.Posts;

/// <summary>
/// Truly minimal amount of data, usefull for sending a list of thousands of posts to place on map.
/// </summary>
public class PostMinimalCategoryRegionDto
{
	// Added this to act as a unique identifier for EF's internal tracking
	public string IdPost { get; set; } = null!;

	public int IdRegion { get; set; }
	public required string Title { get; set; }
	public int IdCategory { get; set; }

	public double Lat { get; set; }
	public double Long { get; set; }


	public static explicit operator PostMinimalCategoryRegionDto?(Post? post)
	{
		return post is null ? null : new PostMinimalCategoryRegionDto
		{
			IdRegion = post.IdRegion,
			Title = post.Title,
			IdCategory = post.IdCategory,
		};
	}
}
