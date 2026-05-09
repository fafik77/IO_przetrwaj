using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.Regions;

public class GetGmiRegionsQuery : IQuery<IEnumerable<RegionOnlyDto>>
{ }
