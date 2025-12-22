namespace PrzetrwajPL.Requests;

public class AddAttachments
{
	public IList<string>? AlternateDescriptions { get; set; }
	public IFormFileCollection? Files { get; set; }
}
