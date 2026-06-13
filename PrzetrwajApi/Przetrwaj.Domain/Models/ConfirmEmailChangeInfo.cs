using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Models;

public record ConfirmEmailChangeInfo : ConfirmEmailInfo
{
	[Required]
	[EmailAddress]
	public required string NewEmail { get; set; }
}
