using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Przetrwaj.Application.Settings;
using Przetrwaj.Domain.Exceptions.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Przetrwaj.Application.Helpers;

public class AuthorizationHelper
{
	private readonly JwtSettings _jwtSettings;

	public AuthorizationHelper(IOptions<JwtSettings> options, bool ValidateLifetime = true)
	{
		_jwtSettings = options.Value;
	}

	public ClaimsPrincipal GetPrincipalClaimsFromTokens(List<string> authorizationTokens, bool ValidateLifetime = true)
	{
		if (authorizationTokens.Count != 1) throw new InvalidAuthorizationException("Invalid authorization token");
		return GetPrincipalClaimsFromToken(authorizationTokens[0], ValidateLifetime);
	}

	public ClaimsPrincipal GetPrincipalClaimsFromToken(string authorizationToken, bool ValidateLifetime = true)
	{
		if (authorizationToken.StartsWith("Bearer ")) authorizationToken = authorizationToken.Substring("Bearer ".Length);

		var tokenValidationParameters = new TokenValidationParameters
		{
			ValidateAudience = true,
			ValidateIssuer = true,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(_jwtSettings.KeyBytes),
			ValidateLifetime = ValidateLifetime,   // We want to get claims from expired token
			ValidAudience = _jwtSettings.Audience,
			ValidIssuer = _jwtSettings.Issuer,
			ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
		};

		var tokenHandler = new JwtSecurityTokenHandler();
		var principal = tokenHandler.ValidateToken(authorizationToken, tokenValidationParameters, out var securityToken);

		if (securityToken is not JwtSecurityToken jwtSecurityToken ||
			!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			throw new SecurityTokenException("Invalid token");

		return principal;
	}

	static public string? GetUserId(ClaimsPrincipal claims) => claims.FindFirstValue(ClaimTypes.NameIdentifier);
	static public string? GetJti(ClaimsPrincipal claims) => claims.FindFirstValue(JwtRegisteredClaimNames.Jti);
}
