using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Posts;

public record UpdatePostCommand
{
    [MaxLength(200)]
    [MinLength(3)]
    public string? Title { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? CustomCategory { get; set; }
}
