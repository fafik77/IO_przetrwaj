using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Settings;

public class AttachmentSettings
{
	[Range(1, 100)]
	public int MaxFiles { get; set; }
	[Range(1, 30)]
	public int MaxFileSizeInMB { get; set; }
	[Required]
	public required IEnumerable<string> AllowedTypes { get; set; }
}

