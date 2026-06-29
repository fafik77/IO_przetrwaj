using Przetrwaj.CommonLibrary.Requests;
using System.Net.Http.Headers;

namespace Przetrwaj.CommonLibrary.Logic;

public class CreateUploadAttachments
{
	public static (string url, MultipartFormDataContent data) CreateData(string id, List<AttachmentItem> items)
	{
		var url = $"Posts/{id}/attachment";
		var content = new MultipartFormDataContent();
		const string itemsPName = nameof(AddAttachments.Items);
		const string filePName = nameof(AttachmentItem.File);
		const string altDescPName = nameof(AttachmentItem.AltDescription);

		for (int i = 0; i < items.Count; i++)
		{
			var item = items[i];

			// 1. Add the File stream
			// We use a high maxAllowedSize (10MB) to prevent WASM stream errors
			var fileStream = item.File.OpenReadStream(10 << 20);
			var fileContent = new StreamContent(fileStream);
			fileContent.Headers.ContentType = new MediaTypeHeaderValue(item.File.ContentType);

			// Name must match the API's property: Items[i].File
			content.Add(fileContent, $"{itemsPName}[{i}].{filePName}", item.File.Name);

			// 2. Add the AltDescription
			if (!string.IsNullOrEmpty(item.AltDescription))
			{
				// Name must match the API's property: Items[i].AltDescription
				content.Add(new StringContent(item.AltDescription), $"{itemsPName}[{i}].{altDescPName}");
			}
		}

		return (url, content);
	}
}