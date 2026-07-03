using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Application.Dtos;

public class PostCreatedDto
{
	public required PostOverviewDto Post { get; set; }
	public AddAttachmentsResult? Attachments { get; set; }

	public static PostCreatedDto Map((Post, AddAttachmentsResult?) res, string httpPath)
	{
		return new PostCreatedDto
		{
			Post = PostOverviewDto.Map(res.Item1),
			Attachments = res.Item2
		};
	}
}