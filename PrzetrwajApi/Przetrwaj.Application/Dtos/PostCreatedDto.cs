using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Application.Dtos;

public class PostCreatedDto
{
	public required PostOverviewDto Post { get; set; }
	public AddAttachmentsResult? Attachments { get; set; }

	public static PostCreatedDto Map((Post, AddAttachmentsResult?) res, Uri httpPath)
	{
		var attachResults = res.Item2;
		if (attachResults != null)
			foreach (var item in attachResults.Attachments)
			{
				item.BaseUrl = httpPath;
			}
		return new PostCreatedDto
		{
			Post = PostOverviewDto.Map(res.Item1),
			Attachments = attachResults
		};
	}
}