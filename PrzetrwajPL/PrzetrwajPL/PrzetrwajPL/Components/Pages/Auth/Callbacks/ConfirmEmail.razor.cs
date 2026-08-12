using Microsoft.AspNetCore.Components;
using Przetrwaj.CommonLibrary.Consts;
using System.Net;

namespace PrzetrwajPL.Components.Pages.Auth.Callbacks;
public partial class ConfirmEmail
{
	private bool _isProcessing = true;
	private bool _isSuccess = false;
	private string _errorMessage = "";
	[SupplyParameterFromQuery] public string? UserId { get; set; }
	[SupplyParameterFromQuery] public string? Code { get; set; }

	protected override async Task OnInitializedAsync()
	{
		if (string.IsNullOrWhiteSpace(UserId) || string.IsNullOrWhiteSpace(Code))
		{
			_errorMessage = "Brakujący identyfikator użytkownika lub kod weryfikacyjny.";
			_isProcessing = false;
			return;
		}

		try
		{
			// Note: Since backend is [HttpGet], we append parameters to the URL
			// We use WebUtility.UrlEncode to ensure characters like '+' in the code don't break the query
			var requestUrl = $"Account/confirm-email?userId={UserId}&code={WebUtility.UrlEncode(Code)}";
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			var response = await client.GetAsync(requestUrl);
			if (response.IsSuccessStatusCode)
			{
				_isSuccess = true;
			}
			else
			{
				// Here we could parse ExceptionCasting object if needed
				_errorMessage = "Link wygasł lub jest nieprawidłowy.";
			}
		}
		catch (Exception)
		{
			_errorMessage = "Nie udało się połączyć z serwerem.";
		}
		finally
		{
			_isProcessing = false;
		}
	}
}