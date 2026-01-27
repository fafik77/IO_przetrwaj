using PrzetrwajPL.Requests;
using System.Net.Http.Headers;

namespace Przetrwaj.CommonLibrary.Logic;

public class CreateUploadAttachments
{
	public static (string url, MultipartFormDataContent data) CreateData(string id, List<AttachmentItem> items)
	{
		// The endpoint is now just the path since descriptions are in the body
		var url = $"Post/{id}/Attachment";
		var content = new MultipartFormDataContent();

		for (int i = 0; i < items.Count; i++)
		{
			var item = items[i];

			// 1. Add the File stream
			// We use a high maxAllowedSize (10MB) to prevent WASM stream errors
			var fileStream = item.File.OpenReadStream(1024 * 1024 * 10);
			var fileContent = new StreamContent(fileStream);
			fileContent.Headers.ContentType = new MediaTypeHeaderValue(item.File.ContentType);

			// Name must match the API's property: Items[i].File
			content.Add(fileContent, $"Items[{i}].File", item.File.Name);

			// 2. Add the AltDescription
			if (!string.IsNullOrEmpty(item.AltDescription))
			{
				// Name must match the API's property: Items[i].AltDescription
				content.Add(new StringContent(item.AltDescription), $"Items[{i}].AltDescription");
			}
		}

		return (url, content);
	}
}