using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Dtos;

public class AttachmentDto
{
	[MaxLength(64)]
	public required string IdAttachment { get; set; }
	[MaxLength(200)]
	public string? AlternateDescription { get; set; }

	//public data stream
}
