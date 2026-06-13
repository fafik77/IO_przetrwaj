using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Przetrwaj.Domain.Entities;

public enum RegionPrecision
{
	PL = 0,
	WOJ = 1,
	POW = 2,
	GMI = 3,
}
public interface IRegionInfo
{
	public int Id { get; }
	public string Name { get; set; }
	public short? ParentId { get; }
	public RegionPrecision Type { get; }
}

//source TERYT: TERC_Urzedowy
public abstract class Region : IRegionInfo
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int Id { get; set; }
	public required string Name { get; set; }

	public int? ParentId { get; set; }
	public virtual Region? Parent { get; set; }
	public virtual ICollection<Region> Children { get; set; } = new List<Region>();

	public virtual ICollection<Post> Posts { get; set; } = new List<Post>();


	public abstract RegionPrecision Type { get; }
	static public readonly string Type_ = "RegionType";
	int IRegionInfo.Id => Id;
	short? IRegionInfo.ParentId => (short?)(ParentId);
}

//Województwo || Polska{id=0}
public class RegionWoj : Region
{
	public override RegionPrecision Type => Id == 0 ? RegionPrecision.PL : RegionPrecision.WOJ;
}
public class RegionPow : Region
{
	public override RegionPrecision Type => RegionPrecision.POW;
}
public class RegionGmi : Region
{
	public override RegionPrecision Type => RegionPrecision.GMI;
}