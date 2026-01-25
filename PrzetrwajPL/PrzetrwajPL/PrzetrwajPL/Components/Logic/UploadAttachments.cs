using PrzetrwajPL.Requests;

namespace PrzetrwajPL.Components.Logic;

class CreateUploadAttachments
{
	public static (string, MultipartFormDataContent) CreateData(string id, AddAttachments request)
	{
		// 1. Construct the URL with Query Parameters for AlternateDescriptions
		// Example: Post/123/Attachment?AlternateDescriptions=desc1&AlternateDescriptions=desc2
		var queryString = new List<string>();
		if (request.AlternateDescriptions != null)
		{
			foreach (var desc in request.AlternateDescriptions)
			{
				queryString.Add($"AlternateDescriptions={Uri.EscapeDataString(desc)}");
			}
		}

		var url = $"Post/{id}/Attachment" + (queryString.Count > 0 ? "?" + string.Join("&", queryString) : "");

		// 2. Prepare the Multipart Content for the Files
		using var content = new MultipartFormDataContent();

		if (request.Files != null)
		{
			foreach (var file in request.Files)
			{
				var fileContent = new StreamContent(file.OpenReadStream());
				content.Add(fileContent, "Files", file.FileName);
			}
		}
		return (url, content);
	}
}