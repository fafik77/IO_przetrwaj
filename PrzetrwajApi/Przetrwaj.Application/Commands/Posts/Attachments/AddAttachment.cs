using Microsoft.AspNetCore.Http;

namespace Przetrwaj.Application.Commands.Posts.Attachments
{
	public class AddAttachment
	{
		public IList<string>? AlternateDescriptions { get; set; }
		public IFormFileCollection? Files { get; set; }
	}
}
