using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PrzetrwajPL.Handlers.Security;

public class JwtAuthProvider : AuthenticationStateProvider
{
	private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());

	public override Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		throw new NotImplementedException();
	}
	public async Task NotifyUserLogin(string Token)
	{
		var readToken = new JwtSecurityTokenHandler().ReadJwtToken(Token);
		var ident = new ClaimsIdentity(readToken.Claims, JwtBearerDefaults.AuthenticationScheme);
		var claims = new ClaimsPrincipal(ident);
		var state = new AuthenticationState(claims);
		NotifyAuthenticationStateChanged(Task.FromResult(state));
	}
}
