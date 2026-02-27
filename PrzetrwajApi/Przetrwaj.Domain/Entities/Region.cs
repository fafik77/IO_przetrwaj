using Przetrwaj.Domain.Models.Dtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Przetrwaj.Domain.Entities;

public enum RegionPrecision
{
	PL,
	WOJ,
	POW,
	GMI
}
public interface IRegionInfo
{
	public int Id { get; }
	public string Name { get; set; }
	public LatLong? LatLong { get; }
	public short ParentId { get; }
}
//source TERYT: TERC_Urzedowy, ULIC_Urzedowy
//Województwo || Polska{id=0}
public class RegionWoj : IRegionInfo
{
	//2x Char
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)] // Id set manualy
	public short Id { get; set; }
	public required string Name { get; set; }

	public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
	public virtual ICollection<RegionPow> Powiaty { get; set; } = [];


	int IRegionInfo.Id => Id * 100_000;
	LatLong? IRegionInfo.LatLong => null;
	public short ParentId => 0;
}
//Powiat
public class RegionPow : IRegionInfo
{
	//2x Char
	//[Key]
	public short WojId { get; set; }
	//4x Char
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)] // Id set manualy 
	public short Id { get; set; }
	public required string Name { get; set; }
	public double Lat { get; set; }
	public double Long { get; set; }

	public virtual RegionWoj Woj { get; set; }
	public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
	public virtual ICollection<AppUser> Users { get; set; } = new List<AppUser>();
	public virtual ICollection<RegionGmi> Gminy { get; set; } = [];

	int IRegionInfo.Id => Id * 1_000;
	LatLong IRegionInfo.LatLong => new LatLong(Lat, Long);
	public short ParentId => WojId;
}
//Gmina
public class RegionGmi : IRegionInfo
{
	//2x Char
	//[Key]
	//public short WojId { get; set; }
	//4x Char
	//[Key]
	public short PowId { get; set; }
	//6(+1)x Char (where last one will be ignored as it violates the Name uniqness)
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)] // Id set manualy
	public int Id { get; set; }
	public required string Name { get; set; }
	public double Lat { get; set; }
	public double Long { get; set; }

	public virtual RegionPow Pow { get; set; }
	public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

	int IRegionInfo.Id => Id * 10;
	LatLong IRegionInfo.LatLong => new LatLong(Lat, Long);
	public short ParentId => PowId;
}
