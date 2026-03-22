using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.AccountOwn;

public record RefreshTokenCommand
{
	[Required]
	public required string RefreshToken { get; set; }
}
public class RefreshTokenInternalCommand : ICommand<JwtTokenDto>
{
	public required string RefreshToken { get; set; }
	public required string UserId { get; set; }
	public required string Jti { get; set; }
}
