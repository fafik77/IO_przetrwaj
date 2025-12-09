using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.AccountOwn;

public class UpdateAccountInternalCommand : UpdateAccountCommand, ICommand<UserWithPersonalDataDto>
{
	public string UserId { get; set; }
}

public class UpdateAccountCommand
{
	[MaxLength(64)]
	public string? Name { get; set; }
	[MaxLength(64)]
	public string? Surname { get; set; }
	[EmailAddress]
	public string? Email { get; set; }
	public int? IdRegion { get; set; }
}
