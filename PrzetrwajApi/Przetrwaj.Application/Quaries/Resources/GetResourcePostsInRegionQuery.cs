using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Models.Dtos.Posts;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Quaries.Resources;

public record GetResourcePostsInRegionQuery : IQuery<IEnumerable<PostOverviewDto>>
{
    [Required]
    public int IdRegion { get; set; }
}
