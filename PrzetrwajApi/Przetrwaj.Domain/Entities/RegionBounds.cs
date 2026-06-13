using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Entities;

public class RegionBounds
{
	[Key]
	public int Id { get; set; }
	//public NetTopologySuite.Geometries.Geometry Boundary { get; set; }
}