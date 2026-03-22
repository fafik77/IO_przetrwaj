using Przetrwaj.Application.Configuration.Commands;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.AccountOwn;

public record LogoutAllSesionsCommand : ICommand
{
	[Required]
	public required string UserId { get; set; }
}

