using Microsoft.AspNetCore.Http;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;

namespace Przetrwaj.Application.Commands.Posts.Attachments;

public record AddAttachmentsInternal : AddAttachments, ICommand<AddAttachmentsResult>
{
	public required string IdPost { get; set; }
	public required string IdUser { get; set; }
}
public class AttachmentItem
{
	public required IFormFile File { get; set; }
	public string? AltDescription { get; set; }
}

public record AddAttachments
{
	// A list of pairs ensures the data stays together
	public required List<AttachmentItem> Items { get; set; }
}
