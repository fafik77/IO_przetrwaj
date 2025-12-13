using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Przetrwaj.Application.Dtos;

public class PostMinimalCategoryRegionDto
{
	public int IdRegion { get; set; }
	public double Lat { get; set; }
	public double Long { get; set; }
	public int IdCategory { get; set; }
	public required string CategoryIcon { get; set; }
	public required string Title { get; set; }

}
