using System.ComponentModel.DataAnnotations;

namespace PrzetrwajPL.Requests.Parts;

public class ConfirmEmailInfo
{
	[Required]
	public required string UserId { get; set; }
	[Required]
	public required string Code { get; set; }
}
