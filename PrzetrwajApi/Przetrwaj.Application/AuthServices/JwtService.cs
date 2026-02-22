using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Przetrwaj.Application.AuthServices;

public class JwtService : IJwtService
{
	private readonly IConfiguration _config;
	public JwtService(IConfiguration config)
	{
		_config = config;
	}

	public string GenerateToken(UserWithPersonalDataDto user)
	{
		//make claims
		var claims = new List<Claim>
		{
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

			new Claim(JwtRegisteredClaimNames.Sub, user.Id),
			new Claim(JwtRegisteredClaimNames.Email, user.Email!),
			new Claim("Name", user.Name ?? string.Empty),
			new Claim("Surname", user.Surname ?? string.Empty),
			new Claim("Region", user.Region?.Id.ToString() ?? string.Empty),
			new Claim("BanInfo", user.BanInfo?.ToString() ?? string.Empty),
		};
		foreach (var role in user.Roles) claims.Add(new Claim(ClaimTypes.Role, role));
		//make token
		return MakeTokenWithClaims(claims);
	}
	//public string GenerateToken(AppUser user, IList<string> roles)
	//{
	//	//make claims
	//	var claims = new List<Claim>
	//	{
	//		new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

	//		new Claim(JwtRegisteredClaimNames.Sub, user.Id),
	//		new Claim(JwtRegisteredClaimNames.Email, user.Email!),
	//		new Claim("Name", user.Name ?? string.Empty),
	//		new Claim("Surname", user.Surname ?? string.Empty),
	//		new Claim("Region", user.IdRegion.ToString()),
	//	};
	//	foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role));
	//	//make token
	//	return MakeTokenWithClaims(claims);
	//}

	private string MakeTokenWithClaims(List<Claim> claims)
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

		var tokenHandler = new JwtSecurityTokenHandler();
		var tokenDesc = new SecurityTokenDescriptor
		{
			Issuer = _config["Jwt:Issuer"],
			Audience = _config["Jwt:Audience"],
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.Now.AddHours(24),
			SigningCredentials = creds
		};

		var token = tokenHandler.CreateToken(tokenDesc);
		return tokenHandler.WriteToken(token);
	}
}
