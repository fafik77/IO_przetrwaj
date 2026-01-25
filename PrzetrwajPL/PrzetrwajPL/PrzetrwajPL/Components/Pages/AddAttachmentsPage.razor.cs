using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Przetrwaj.CommonLibrary.Logic;

namespace PrzetrwajPL.Components.Pages
{
	public partial class AddAttachmentsPage
	{
		[Parameter, SupplyParameterFromQuery(Name = "post")]
		public string? PostId { get; set; }

		private List<IBrowserFile> selectedFiles = new();
		private List<string> descriptions = new();
		private bool isUploading = false;
		private string? message;

		private void LoadFiles(InputFileChangeEventArgs e)
		{
			//selectedFiles.Clear();
			//descriptions.Clear();

			// Only allow image types, including gifs 
			var imageFiles = e.GetMultipleFiles(10)
							  .Where(f => f.ContentType.StartsWith("image/"));

			foreach (var file in imageFiles)
			{
				selectedFiles.Add(file);
				descriptions.Add(string.Empty); // Matches index with selectedFiles 
			}

			if (!selectedFiles.Any())
			{
				message = "B³¹d: Wybrano nieprawid³owe pliki. Wybierz tylko obrazy.";
			}
		}

		private async Task UploadAll()
		{
			if (string.IsNullOrEmpty(PostId) || selectedFiles.Count == 0) return;

			isUploading = true;
			message = "Trwa przesy³anie...";
			MultipartFormDataContent? data = null;
			try
			{
				var client = ClientFactory.CreateClient("ServerAPI");

				// Calling helper class with indexed-matched lists 
				(string url, data) =
					CreateUploadAttachments.CreateData(PostId, descriptions, selectedFiles);

				var response = await client.PostAsync(url, data);

				if (response.IsSuccessStatusCode)
				{
					message = "Zdjêcia zosta³y pomyœlnie dodane!";
					await Task.Delay(1500);
					Nav.NavigateTo($"/post/{PostId}");
				}
				else
				{
					message = $"B³¹d: {response.StatusCode}";
				}
			}
			catch (Exception ex)
			{
				message = $"B³¹d: {ex.Message}";
			}
			finally
			{
				data?.Dispose();
				isUploading = false;
			}
		}
	}
}