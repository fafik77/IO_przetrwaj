using Przetrwaj.Domain.Entities;
using System.Text.Json.Serialization;

namespace Przetrwaj.Domain.Models.Dtos;

public record AttachmentDto
{
	public string? AlternateDescription { get; set; }
	public string FileURL { get => $"{BaseUrl}/Attachments/{fileName}"; }

	private string fileName = string.Empty;
	[JsonIgnore]
	public string BaseUrl { get; set; } = string.Empty;

	public static AttachmentDto? Map(Attachment? attachment, string baseUrl)
	{
		return attachment is null ? null : new AttachmentDto
		{
			fileName = attachment.FileName,
			BaseUrl = baseUrl,
			AlternateDescription = attachment.AlternateDescription
		};
	}
}
