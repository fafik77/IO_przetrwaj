using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Models;

public record ConfirmEmailInfo
{
	[Required]
	public required string UserId { get; set; }
	[Required]
	public required string Code { get; set; }
}
