using System.ComponentModel.DataAnnotations;

namespace PrzetrwajPL.Requests.Parts;

public class ConfirmEmailChangeInfo : ConfirmEmailInfo
{
	[Required]
	[EmailAddress]
	public required string NewEmail { get; set; }
}
