using Przetrwaj.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Impediments;

public class EditImpediment
{
	[Required]
	[Range(0, 31)]
	public short Id { get; set; }
	[Required]
	[Length(3, 100)]
	public required string Name { get; set; }

	public Impediment Map()
	{
		return new Impediment { Id = Id, Name = Name };
	}
}
