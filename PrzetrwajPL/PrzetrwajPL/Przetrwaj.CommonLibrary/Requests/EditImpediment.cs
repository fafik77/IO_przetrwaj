using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.CommonLibrary.Requests;

public class EditImpediment
{
	[Required]
	[Range(0, 31)]
	public short Id { get; set; }
	[Required]
	[Length(3, 100)]
	public required string Name { get; set; }
}
