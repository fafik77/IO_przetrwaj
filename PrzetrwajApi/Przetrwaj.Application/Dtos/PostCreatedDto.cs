using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Application.Dtos;

public class PostCreatedDto
{
    public required PostOverviewDto Post { get; set; }
    public AddAttachmentsResult? Attachments { get; set; }

    public static PostCreatedDto Map((Post, AddAttachmentsResult?) res)
    {
        var attachResults = res.Item2;
        return new PostCreatedDto
        {
            Post = PostOverviewDto.Map(res.Item1, 0, 0),
            Attachments = attachResults
        };
    }
}