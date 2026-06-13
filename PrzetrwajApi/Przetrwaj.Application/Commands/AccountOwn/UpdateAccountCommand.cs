using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.AccountOwn;

public interface IUpdateAccountCommand { }

public record UpdateAccountCommand : IUpdateAccountCommand
{
	[MaxLength(64)]
	public string? Name { get; set; }
	[MaxLength(64)]
	public string? Surname { get; set; }
	public int? GminaId { get; set; }
	public int? Impediments { get; set; }
}
