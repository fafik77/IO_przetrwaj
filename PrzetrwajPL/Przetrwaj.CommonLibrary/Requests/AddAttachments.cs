using Microsoft.AspNetCore.Components.Forms;

namespace PrzetrwajPL.Requests;

public class AttachmentItem
{
	public required IBrowserFile File { get; set; }
	public string? AltDescription { get; set; }
}

public class AddAttachments
{
	// A list of pairs ensures the data stays together
	public required List<AttachmentItem> Items { get; set; }
}
