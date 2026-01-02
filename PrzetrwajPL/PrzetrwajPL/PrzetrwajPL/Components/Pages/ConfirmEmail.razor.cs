using Microsoft.AspNetCore.Components;
using System.Net;

namespace PrzetrwajPL.Components.Pages;
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
			_errorMessage = "Brakuj¹cy identyfikator u¿ytkownika lub kod weryfikacyjny.";
			_isProcessing = false;
			return;
		}

		try
		{
			// Note: Since backend is [HttpGet], we append parameters to the URL
			// We use WebUtility.UrlEncode to ensure characters like '+' in the code don't break the query
			var requestUrl = $"Account/ConfirmEmail?userId={UserId}&code={WebUtility.UrlEncode(Code)}";
			var response = await HttpClient.GetAsync(requestUrl);
			if (response.IsSuccessStatusCode)
			{
				_isSuccess = true;
			}
			else
			{
				// Here we could parse ExceptionCasting object if needed
				_errorMessage = "Link wygas³ lub jest nieprawid³owy.";
			}
		}
		catch (Exception)
		{
			_errorMessage = "Nie uda³o siê po³¹czyæ z serwerem.";
		}
		finally
		{
			_isProcessing = false;
		}
	}
}