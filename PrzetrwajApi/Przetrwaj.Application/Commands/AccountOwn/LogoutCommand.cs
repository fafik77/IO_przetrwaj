using Przetrwaj.Application.Configuration.Commands;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.AccountOwn;

public class LogoutCommand : ICommand
{
	[Required]
	public required string UserId { get; set; }
	[Required]
	public required string TokenId { get; set; }
}
