using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Quaries.Users;

public class GetUserByIdQuery : IQuery<UserGeneralDto>
{
	[Required]
	public required string UserId { get; set; }
}
