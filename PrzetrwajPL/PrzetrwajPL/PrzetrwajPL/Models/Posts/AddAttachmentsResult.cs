namespace PrzetrwajPL.Models.Posts;

public class AddAttachmentsResult : ExceptionCasting
{
	public List<AddAttachmentResult> Results { get; set; }
	public IEnumerable<AttachmentDto> Attachments { get; set; }

	public AddAttachmentsResult() : base()
	{
		Results = [];
		Attachments = [];
	}
}
public class AddAttachmentResult : ExceptionCasting
{
	public AttachmentDto? AttachmentDto { get; set; }
	public AddAttachmentResult() : base() { }
}