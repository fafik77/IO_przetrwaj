using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Przetrwaj.Domain.Entities;

public class Impediment
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)] // Id set manualy (0-31)
	public short Id { get; set; }
	public required string Name { get; set; }
}
