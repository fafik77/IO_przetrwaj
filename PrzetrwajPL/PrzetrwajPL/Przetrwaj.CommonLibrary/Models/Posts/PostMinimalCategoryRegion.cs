namespace PrzetrwajPL.Models.Posts;

/// <summary>
/// Truly minimal amount of data, usefull for sending a list of thousands of posts to place on map.
/// </summary>
public class PostMinimalCategoryRegion
{
	// Added this to act as a unique identifier for EF's internal tracking
	public string IdPost { get; set; } = null!;

	public int IdRegion { get; set; }
	public required string Title { get; set; }
	public int IdCategory { get; set; }

	public double Lat { get; set; }
	public double Long { get; set; }
	public bool Active { get; set; }
}
