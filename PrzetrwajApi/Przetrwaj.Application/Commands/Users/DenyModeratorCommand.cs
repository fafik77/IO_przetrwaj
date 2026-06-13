using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Users;

public class DenyModeratorCommand : ICommand<AppUser>
{
	[Required]
	public required string UserIdOrEmail { get; set; }
}
