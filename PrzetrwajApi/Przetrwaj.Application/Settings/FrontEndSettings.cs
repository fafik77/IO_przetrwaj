using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Settings;

public class FrontEndSettings
{
	[Required]
	public required string Url { get; set; }
}
