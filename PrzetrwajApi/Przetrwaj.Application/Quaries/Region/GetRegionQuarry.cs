using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Przetrwaj.Application.Quaries.Region;

public class GetRegionQuarry : IQuery<RegionOnlyDto>
{
	public int IdRegion;
}
