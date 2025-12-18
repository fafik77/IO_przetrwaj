using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos;

public class AttachmentDto
{
	//[MaxLength(64)]
	//public required string IdAttachment { get; set; }
	//[MaxLength(200)]
	public string? AlternateDescription { get; set; }
	//public required string FileNameWithExt { get; set; }
	public required string FileURL { get; set; }
	//public required Stream File {  get; set; }

	public static AttachmentDto? MapFromEntity(Attachment? attachment, string baseUrl)
	{
		return attachment is null ? null : new AttachmentDto
		{
			AlternateDescription = attachment.AlternateDescription,
			FileURL = $"{baseUrl}/Attachments/{attachment.IdAttachment}.webp"
		};
	}

	public static AttachmentDto? MapFromEntity(AttachmentDto? attachment, string baseUrl)
	{
		return attachment is null ? null : new AttachmentDto
		{
			AlternateDescription = attachment.AlternateDescription,
			FileURL = $"{baseUrl}{attachment.FileURL}"
		};
	}
}
