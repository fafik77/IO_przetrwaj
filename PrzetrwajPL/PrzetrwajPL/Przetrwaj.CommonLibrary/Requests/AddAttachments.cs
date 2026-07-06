using Microsoft.AspNetCore.Components.Forms;
using Przetrwaj.CommonLibrary.Extensions;
using System.Net.Http.Headers;

namespace Przetrwaj.CommonLibrary.Requests;

public class AttachmentItem : IMultipartFormDataCreator
{
	public required IBrowserFile File { get; set; }
	public string? AltDescription { get; set; }


	public MultipartFormDataContent ToMultipartData(MultipartFormDataContent multipartFormData, string? rootPath)
	{
		// 1. Add the File stream
		// We use a high maxAllowedSize (10MB) to prevent WASM stream errors
		var fileStream = File.OpenReadStream(10 << 20);
		var fileContent = new StreamContent(fileStream);
		fileContent.Headers.ContentType = new MediaTypeHeaderValue(File.ContentType);

		// Name must match the API's property: Items[i].File
		multipartFormData.Add(fileContent, name: $"{rootPath}.{nameof(File)}", fileName: File.Name);
		// 2. Add the AltDescription
		// Name must match the API's property: Items[i].AltDescription
		multipartFormData.AddStringContent(new(nameof(AltDescription), AltDescription), rootPath);

		return multipartFormData;
	}
}

public class AddAttachments : IMultipartFormDataCreator
{
	// A list of pairs ensures the data stays together (as we have more than one field we need to use a key to combine them into a single instance)
	public required List<AttachmentItem> Items { get; set; }


	public MultipartFormDataContent ToMultipartData(MultipartFormDataContent multipartFormData, string? rootPath)
	{
		var itemsPName = nameof(Items);
		int itemN = 0;
		foreach (var item in Items)
		{
			//we have to include unique index for File<=>AltDescription binding
			multipartFormData.AddContent($"{itemsPName}[{itemN++}]", item, rootPath);
		}
		return multipartFormData;
	}
}
