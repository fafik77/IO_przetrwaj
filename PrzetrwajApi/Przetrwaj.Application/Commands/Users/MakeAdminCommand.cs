using Microsoft.AspNetCore.Identity;
using Przetrwaj.Application.Configuration.Commands;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Users;

public class MakeAdminCommand
{
	[Required]
	public required string UserIdOrEmail { get; set; }
	[Required]
	[PasswordPropertyText]
	public required string Password { get; set; }
}
public class MakeAdminInternallCommand : MakeAdminCommand, ICommand<IdentityResult>
{
	public required string Id { get; set; }
}
