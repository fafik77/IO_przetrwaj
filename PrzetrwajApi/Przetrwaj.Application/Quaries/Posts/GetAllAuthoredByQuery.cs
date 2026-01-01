using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Models.Dtos.Posts;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Quaries.Posts;

public class GetAllAuthoredByQuery : IQuery<IEnumerable<PostOverviewDto>>
{
	[Required]
	public required string AutorId { get; set; }
}
