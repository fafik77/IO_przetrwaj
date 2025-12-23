using Microsoft.AspNetCore.Http;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Posts.Attachments;

public class AddAttachmentsInternal : AddAttachments, ICommand<AddAttachmentsResult>
{
	public required string IdPost { get; set; }
	public required string IdUser { get; set; }
}
public class AddAttachments
{
	public IList<string>? AlternateDescriptions { get; set; }
	public IFormFileCollection? Files { get; set; }
}
