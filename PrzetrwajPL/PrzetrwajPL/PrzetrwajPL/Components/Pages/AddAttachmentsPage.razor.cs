using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Logic;
using Przetrwaj.CommonLibrary.Requests;

namespace PrzetrwajPL.Components.Pages
{
	public partial class AddAttachmentsPage
	{
		[Parameter, SupplyParameterFromQuery(Name = "post")]
		public string? PostId { get; set; }

		private List<AttachmentItem> Attachments = new();
		private bool isUploading = false;
		private string? message;

		private void LoadFiles(InputFileChangeEventArgs e)
		{
			//Attachments.Clear();
			message = null;

			// Filter for image types only
			var files = e.GetMultipleFiles(10).Where(f => f.ContentType.StartsWith("image/"));

			foreach (var file in files)
			{
				if (Attachments.Count >= 10) break;
				Attachments.Add(new AttachmentItem { File = file, AltDescription = string.Empty });
			}
		}

		private async Task UploadAll()
		{
			if (string.IsNullOrEmpty(PostId) || Attachments.Count == 0) return;

			isUploading = true;
			try
			{
				var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);

				// Pass the single unified list to the logic helper
				(string url, MultipartFormDataContent data) = CreateUploadAttachments.CreateData(PostId, Attachments);

				var response = await client.PostAsync(url, data);

				if (response.IsSuccessStatusCode)
				{
					Nav.NavigateTo($"/post/{PostId}");
				}
				else
				{
					message = $"B³¹d serwera: {response.StatusCode}";
				}
			}
			catch (Exception ex)
			{
				message = $"B³¹d po³¹czenia: {ex.Message}";
			}
			finally
			{
				isUploading = false;
			}
		}
	}
}