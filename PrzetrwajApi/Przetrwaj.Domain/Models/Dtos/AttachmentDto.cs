using Przetrwaj.Domain.Consts;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos;

public record AttachmentDto
{
    public string? AlternateDescription { get; set; }
    public string FileURL { get => $"{FileConsts.AttachmentsDirectory}/{fileName}"; }

    private string fileName = string.Empty;

    public static AttachmentDto? Map(Attachment? attachment)
    {
        return attachment is null ? null : new AttachmentDto
        {
            fileName = attachment.FileName,
            AlternateDescription = attachment.AlternateDescription
        };
    }
}
