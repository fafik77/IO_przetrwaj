using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Models.Dtos.Posts;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Quaries.Dangers;

public record GetDangerPostsInRegionQuery : IQuery<IEnumerable<PostOverviewDto>>
{
    [Required]
    public int IdRegion { get; set; }
}
