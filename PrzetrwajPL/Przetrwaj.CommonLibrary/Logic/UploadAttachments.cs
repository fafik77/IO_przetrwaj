using Microsoft.AspNetCore.Components.Forms;

namespace Przetrwaj.CommonLibrary.Logic;

public class CreateUploadAttachments
{
	public static (string, MultipartFormDataContent) CreateData(string id, IEnumerable<string> descriptions, IEnumerable<IBrowserFile> files)
	{
		// 1. Construct the URL with Query Parameters for AlternateDescriptions
		// Example: Post/123/Attachment?Alt=desc1&Alt=desc2
		var queryParams = string.Join("&", descriptions.Select(d => $"Alt={Uri.EscapeDataString(d)}"));

		var url = $"Post/{id}/Attachment?{queryParams}";

		// 2. Prepare the Multipart Content for the Files
		var content = new MultipartFormDataContent();

		foreach (var file in files)
		{
			var fileContentStream = new StreamContent(file.OpenReadStream(maxAllowedSize: 1024 * 1024 * 10));   // 10 MB limit per file
			fileContentStream.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
			content.Add(fileContentStream, "Files", file.Name);
		}

		return (url, content);
	}
}