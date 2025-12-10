using Microsoft.AspNetCore.Identity;
using Przetrwaj.Application.Configuration.Commands;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Users;

public class MakeModeratorCommand : ICommand<IdentityResult>
{
	[Required]
	public required string UserIdOrEmail { get; set; }
}
