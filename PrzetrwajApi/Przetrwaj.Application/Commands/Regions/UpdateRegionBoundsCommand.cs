using Microsoft.AspNetCore.Http;
using Przetrwaj.Application.Configuration.Commands;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Regions;

public class UpdateRegionBoundsCommand : ICommand<UpdateRegionBoundsResults>
{
	[Required]
	public required IFormFile File { get; set; }
}
