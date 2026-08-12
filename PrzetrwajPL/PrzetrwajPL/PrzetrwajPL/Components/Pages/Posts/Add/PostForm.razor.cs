using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Extensions;
using Przetrwaj.CommonLibrary.Requests;
using System.Text.RegularExpressions;

namespace PrzetrwajPL.Components.Pages.Components;

public partial class PostForm
{
	[Parameter]
	public string PostType { get; set; } = string.Empty;
	public string PostApiEndpoint => "/Posts/" + PostType;
	public string PostTypePlural => PostType + "s";
	[CascadingParameter]
	private HttpContext? HttpContext { get; set; }


	private const string getUserLocationJsName = "getUserLocation";
	private const string initializeLocationPickerMapJsName = "initializeLocationPickerMap";
	private const string locationPickerMapId = "locationPickerMap";
	private const string setLocationOnMapJsName = "setLocationOnMap";
	private string PostTypeAboutPl => PostType.Equals("resource", StringComparison.OrdinalIgnoreCase) ? "zasobie" : "zagro�eniu";

	private EditContext? editContext;
	private ValidationMessageStore? messageStore;
	private CategoryPicker? CategoryPicker;
	private VisibilityPickerSingle? visibilityPickerSingle;
	private ImageAttachments? ImageAttachments;

	private readonly AddPostCommand model = new() { Title = "" };
	private string? error;
	private string? newPostId;

	private LatLong? latLong;
	private bool showMap = false;
	private DotNetObjectReference<PostForm>? objRef;

	private static readonly Regex InneCategoryRegex = new Regex(
		@"^(inne|inna)(\s+\w+)?$",
		RegexOptions.Compiled | RegexOptions.IgnoreCase
	);

	protected override void OnInitialized()
	{
		editContext = new EditContext(model);
		messageStore = new ValidationMessageStore(editContext);
		editContext.OnFieldChanged += (s, e) => messageStore.Clear(e.FieldIdentifier);
		objRef = DotNetObjectReference.Create(this);
	}

	#region JS 
	private async Task HandleUseMyLocation()
	{
		error = null;
		await JS.InvokeVoidAsync(getUserLocationJsName, objRef);
	}

	private async Task HandleShowMapPicker()
	{
		error = null;
		showMap = true;
		//force refresh and delay it till its applied
		StateHasChanged();
		await Task.Delay(1); //this delay is crucial, otherwise the map is not displayed in the div
		await JS.InvokeVoidAsync(initializeLocationPickerMapJsName, locationPickerMapId, objRef);
	}

	/// <summary>
	/// method called from JS, sets the coordinates
	/// </summary>
	/// <param name="lat">latitude</param>
	/// <param name="lng">longitude</param>
	[JSInvokable]
	public async Task SetLocation(double lat, double lng)
	{
		model.LatLong.Lat = lat;
		model.LatLong.Long = lng;
		latLong = model.LatLong;
		//place the marker on the map
		await JS.InvokeVoidAsync(setLocationOnMapJsName, locationPickerMapId, lat, lng);
		StateHasChanged();
	}

	/// <summary>
	/// method called from JS, sets the error message
	/// </summary>
	/// <param name="errorMessage">error message</param>
	[JSInvokable]
	public void LocationError(string errorMessage)
	{
		error = errorMessage;
		StateHasChanged();
	}

	#endregion //JS

	private async Task HandleSubmit()
	{
		if (!string.IsNullOrEmpty(newPostId)) return; //already added a post
		messageStore?.Clear();
		error = null;

		if (string.IsNullOrEmpty(model.Title))
		{
			error = "Post musi mie� tytu�";
			return;
		}
		else if (model.IdCategory <= 0)
		{
			error = "Post musi przynale�e� do kategori";
			return;
		}
		else if (InneCategoryRegex.IsMatch(CategoryPicker?.SelectedCategoryName ?? string.Empty))
		{
			if (string.IsNullOrWhiteSpace(model.CustomCategory) || model.CustomCategory.Length < 3 || model.CustomCategory.Length > 100)
			{
				error = "W�asna kategoria musi mie� od 3 do 100 znak�w";
				messageStore?.Add(() => model.CustomCategory ?? "", error);
				editContext?.NotifyValidationStateChanged();
				return;
			}
		}
		else if (latLong == null)
		{
			error = "Lokalizacja zdarzenia jest wymagana.";
			return;
		}

		if (visibilityPickerSingle != null)
			model.RegionPrecision = visibilityPickerSingle.visibility;
		if (ImageAttachments != null)
			model.Attachments = new AddAttachments { Items = ImageAttachments.Items };

		try
		{
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			using var multipartModelData = new MultipartFormDataContent().AddContent(model);
			var response = await client.PostAsync(PostApiEndpoint, multipartModelData);

			if (response.IsSuccessStatusCode)
			{
				newPostId = null;
				var location = response.Headers.Location;
				if (location != null)
				{
					//this locks the ability to add it again
					newPostId = location.Segments.Last().TrimEnd('/');
					var redirectUrl = $"posts/{newPostId}";
					if (HttpContext?.Response != null)
						HttpContext.Response.Redirect(redirectUrl);
					else
						Nav.NavigateTo(redirectUrl);
				}
			}
			else
			{
				var msg = await response.Content.ReadAsStringAsync();
				error = $"Nie uda�o si� doda� posta. Kod: {response.StatusCode}, odpowied�: {msg}";
			}
		}
		catch (Exception ex)
		{
			error = $"B��d po��czenia: {ex.Message}";
		}
	}

	public void Dispose()
	{
		objRef?.Dispose();
	}
}