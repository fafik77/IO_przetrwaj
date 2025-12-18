using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Models.Dtos.Posts;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Quaries.Posts;

public class GetPostByIdQuery : IQuery<PostCompleteDataDto>
{
	[Required]
	public required string Id { get; set; }
}
