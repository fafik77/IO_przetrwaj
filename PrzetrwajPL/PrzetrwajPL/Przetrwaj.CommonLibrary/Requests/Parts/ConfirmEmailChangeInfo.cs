using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.CommonLibrary.Requests.Parts;

public class ConfirmEmailChangeInfo : ConfirmEmailInfo
{
	[Required]
	[EmailAddress]
	public required string NewEmail { get; set; }
}
