using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Models;

public class ConfirmEmailChangeInfo : ConfirmEmailInfo
{
	[Required]
	[EmailAddress]
	public required string NewEmail { get; set; }
}
