using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Queries.Regions;

public class GetRegionsQuarry : IQuery<IEnumerable<RegionOnlyDto>>
{
	public string? NameLike { get; set; }
	public RegionPrecision? Precision { get; set; }
	public int? ParentId { get; set; }
}
