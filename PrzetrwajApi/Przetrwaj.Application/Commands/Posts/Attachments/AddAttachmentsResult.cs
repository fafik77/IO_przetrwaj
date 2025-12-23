using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions._base;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Posts.Attachments;

public class AddAttachmentsResult : ExceptionCasting
{
	public List<AddAttachmentResult> Results { get; set; }
	public IEnumerable<AttachmentDto> Attachments { get; set; }

	public AddAttachmentsResult() : base()
	{
		Results = [];
		Attachments = [];
	}

	private AddAttachmentsResult(ExceptionCasting baseData) : base(baseData)
	{
		Results = [];
		Attachments = [];
	}

	public static explicit operator AddAttachmentsResult(BaseException exception) =>
		new((ExceptionCasting)exception);
}
public class AddAttachmentResult : ExceptionCasting
{
	public AttachmentDto? AttachmentDto { get; set; }
	public AddAttachmentResult() : base() { }

	private AddAttachmentResult(ExceptionCasting baseData) : base(baseData)
	{ }
	public static explicit operator AddAttachmentResult(BaseException exception) =>
		new((ExceptionCasting)exception);
}