using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.AccountOwn;

public record ForgotPasswordCommand : ICommand<UserGeneralDto> 
{
	[Required]
	[EmailAddress]
	public required string Email { get; set; }
}
