using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Quaries.Impediments;
public class GetImpedimentQuery : IQuery<Impediment>
{
	[Required]
	[Range(0, 31)]
	public short Id { get; set; }
}
