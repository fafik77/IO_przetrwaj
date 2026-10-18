using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Models;
using Przetrwaj.CommonLibrary.Requests;


namespace PrzetrwajPL.Components.Pages.Auth.Login;

public partial class LoginForm : IDisposable
{
    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    [SupplyParameterFromForm]
    public LoginRequest LoginRequest { get; set; } = new LoginRequest();
    private string errorMessage = string.Empty;
    private bool isLoading = false;
    private DotNetObjectReference<LoginForm>? _dotNetHelper;
    const string googleAuthJsName = "googleAuth";
    const string googleAuthJsInit = "init";
    const string triggerGoogleSignIn = "triggerGoogleSignIn";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetHelper = DotNetObjectReference.Create(this);
            var googleClientId = Configuration["OAuth:Google:ClientId"];
            await JSRuntime.InvokeVoidAsync($"{googleAuthJsName}.{googleAuthJsInit}", googleClientId, _dotNetHelper, nameof(OnGoogleLoginSuccess));
        }
    }

    [JSInvokable]
    public async Task OnGoogleLoginSuccess(string idToken)
    {
        isLoading = true;
        errorMessage = string.Empty;
        StateHasChanged();
        try
        {
            var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
            var response = await client.PostAsJsonAsync("/Login/google", new GoogleLoginRequest { IdToken = idToken });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JwtTokenDto>();
                var queryParams = new Dictionary<string, string?>
                {
                    { "token", result!.Token },
                    { "refreshToken", result.RefreshToken }
                };
                var redirectUrl = QueryHelpers.AddQueryString("/account/signin", queryParams);
                if (HttpContext?.Response != null)
                    HttpContext.Response.Redirect(redirectUrl);
                else
                    NavigationManager.NavigateTo(redirectUrl, forceLoad: true);
            }
            else if (response.StatusCode == (System.Net.HttpStatusCode)StatusCodes.Status418ImATeapot)
            {
                var banData = await response.Content.ReadFromJsonAsync<BanInfo>();
                if (banData != null)
                {
                    string jsonString = System.Text.Json.JsonSerializer.Serialize(banData);
                    NavigationManager.NavigateTo($"/login-error/Banned?Info={Uri.EscapeDataString(jsonString)}");
                }
            }
            else
            {
                errorMessage = "Błąd logowania przez konto Google.";
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Błąd połączenia: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        _dotNetHelper?.Dispose();
    }

    private async Task HandleLogin()
    {
        isLoading = true;
        errorMessage = string.Empty;
        try
        {
            var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
            var response = await client.PostAsJsonAsync("/Login/email", LoginRequest);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JwtTokenDto>();
                // Gather all parameters securely to prevent URL corruption when passing parrameters
                var queryParams = new Dictionary<string, string?>
                {
                    { "token", result!.Token },
                    { "refreshToken", result.RefreshToken }
                };

                var redirectUrl = QueryHelpers.AddQueryString("/account/signin", queryParams);

                // Safe execution checking for SSR vs Interactive Server Circuit
                if (HttpContext?.Response != null)
                {
                    HttpContext.Response.Redirect(redirectUrl);
                }
                else
                {
                    // Force a true page load so the AccountController can establish a standard HTTP Cookie context
                    NavigationManager.NavigateTo(redirectUrl, forceLoad: true);
                }
            }
            else if (response.StatusCode == (System.Net.HttpStatusCode)StatusCodes.Status418ImATeapot)
            {
                var banData = await response.Content.ReadFromJsonAsync<BanInfo>();

                if (banData != null)
                {
                    string jsonString = System.Text.Json.JsonSerializer.Serialize(banData);
                    string secureQueryParam = Uri.EscapeDataString(jsonString);
                    NavigationManager.NavigateTo($"/login-error/Banned?Info={secureQueryParam}");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorText = await response.Content.ReadFromJsonAsync<ExceptionCasting>();
                errorMessage = "Nieprawidłowy email lub hasło.";
            }
            else
            {
                errorMessage = "Nieprawidłowy email lub hashasłoo.";
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Błąd połączenia: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }
}
