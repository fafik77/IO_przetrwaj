using Microsoft.AspNetCore.Components;
using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Logic;
using PrzetrwajPL.Components.Pages.Components;

namespace PrzetrwajPL.Components.Pages;

public partial class AddAttachmentsPage
{
	[Parameter]
	public string? PostId { get; set; }

	private ImageAttachments? attachmentsComponent;
	private bool isUploading = false;
	private string? message;

	private async Task UploadAll()
	{
		var items = attachmentsComponent?.Items;
		if (string.IsNullOrEmpty(PostId) || items == null || items.Count == 0) return;

		isUploading = true;
		try
		{
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);

			// Pass the single unified list to the logic helper
			(string url, MultipartFormDataContent data) = CreateUploadAttachments.CreateData(PostId, items);

			var response = await client.PostAsync(url, data);

			if (response.IsSuccessStatusCode)
			{
				Nav.NavigateTo($"/post/{PostId}");
			}
			else
			{
				message = $"Błąd serwera: {response.StatusCode}";
			}
		}
		catch (Exception ex)
		{
			message = $"Błąd połączenia: {ex.Message}";
		}
		finally
		{
			isUploading = false;
		}
	}
}
