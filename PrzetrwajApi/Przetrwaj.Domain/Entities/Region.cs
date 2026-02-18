using Przetrwaj.Domain.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Entities;

public class Region
{
	[Key]
	public int IdRegion { get; set; }
	//[MaxLength(100)]
	public required string Name { get; set; }
	public double Lat { get; set; }
	public double Long { get; set; }


	public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

	public virtual ICollection<AppUser> Users { get; set; } = new List<AppUser>();
}


public interface IRegionInfo
{
	public int Id { get; }
	public string Name { get; }
	public LatLong? LatLong { get; }
}
//source TERYT: TERC_Urzedowy, ULIC_Urzedowy
//Województwo || Polska{id=0}
public class RegionWoj : IRegionInfo
{
	//2x Char
	[Key]
	public short Id { get; set; }
	public required string Name { get; set; }

	public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
	public virtual ICollection<RegionPow> Powiaty { get; set; } = [];

	int IRegionInfo.Id => Id * 100000;
	LatLong? IRegionInfo.LatLong => null;
}
//Powiat
public class RegionPow : IRegionInfo
{
	//2x Char
	//[Key]
	public short WojId { get; set; }
	//4x Char
	[Key]
	public short Id { get; set; }
	public required string Name { get; set; }
	public double Lat { get; set; }
	public double Long { get; set; }

	public virtual required RegionWoj Woj { get; set; }
	public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
	public virtual ICollection<AppUser> Users { get; set; } = new List<AppUser>();
	public virtual ICollection<RegionGmi> Gminy { get; set; } = [];
	//public virtual ICollection<RegionUl> Ulice { get; set; } = [];

	int IRegionInfo.Id =>  Id * 1000;
	LatLong IRegionInfo.LatLong => new LatLong(Lat, Long);
}
//Gmina/Dzielnica
public class RegionGmi : IRegionInfo
{
	//2x Char
	//[Key]
	//public short WojId { get; set; }
	//4x Char
	//[Key]
	public short PowId { get; set; }
	//6+1x Char (where last one will be ignored as it violates the Name uniqness)
	[Key]
	public int Id { get; set; }
	public required string Name { get; set; }
	public double Lat { get; set; }
	public double Long { get; set; }

	public virtual required RegionPow Pow { get; set; }
	public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

	int IRegionInfo.Id => Id * 10;
	LatLong IRegionInfo.LatLong => new LatLong(Lat, Long);
}
//Ulica
//public class RegionUl
//{
//	//[Key]
//	//public short WojId { get; set; }
//	[Key]
//	public short PowId { get; set; }
//	//SYM_UL
//	[Key]
//	public int Id { get; set; }
//	public required string Name { get; set; }

//	public virtual required RegionPow Pow { get; set; }
//	public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
//}
