using Microsoft.AspNetCore.Components;
using Przetrwaj.CommonLibrary.Consts;
using System.Net;

namespace PrzetrwajPL.Components.Pages.Auth.Callbacks;
public partial class ConfirmEmailChange
{
	private enum LoadingStatus { Processing, Success, Error }
	private LoadingStatus _status = LoadingStatus.Processing;
	private string _errorMessage = "";

	[SupplyParameterFromQuery] public string? UserId { get; set; }
	[SupplyParameterFromQuery] public string? Code { get; set; }
	[SupplyParameterFromQuery] public string? NewEmail { get; set; }

	protected override async Task OnInitializedAsync()
	{
		if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(Code) || string.IsNullOrEmpty(NewEmail))
		{
			_status = LoadingStatus.Error;
			_errorMessage = "Nieprawid�owy link zmiany adresu e-mail.";
			return;
		}

		try
		{
			// Note: Since backend is [HttpGet], we append parameters to the URL
			// We use WebUtility.UrlEncode to ensure characters like '+' in the code don't break the query
			var url = $"Account/ConfirmEmailChange?userId={UserId}&newEmail={NewEmail}&code={WebUtility.UrlEncode(Code)}";
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			var response = await client.GetAsync(url);
			if (response.IsSuccessStatusCode)
			{
				_status = LoadingStatus.Success;
			}
			else
			{
				_status = LoadingStatus.Error;
				_errorMessage = "Link wygas� lub zosta� ju� wykorzystany.";
			}
		}
		catch (Exception)
		{
			_status = LoadingStatus.Error;
			_errorMessage = "Wyst�pi� b��d po��czenia z serwerem.";
		}
	}
}