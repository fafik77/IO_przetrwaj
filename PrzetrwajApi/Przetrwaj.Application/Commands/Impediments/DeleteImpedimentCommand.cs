using Przetrwaj.Application.Configuration.Commands;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Impediments;

public class DeleteImpedimentCommand : ICommand
{
	[Required]
	[Range(0, 31)]
	public short Id { get; set; }
}
