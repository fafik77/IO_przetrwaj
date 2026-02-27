using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Przetrwaj.Application.AuthServices;

public class JwtService : IJwtService
{
	private readonly IConfiguration _config;
	private readonly ILogoutCache _logoutCache;
	private readonly IUserJwtRefreshRepository _userJwtRefreshRepository;
	private readonly IUnitOfWork _unitOfWork;

	public JwtService(IConfiguration config, ILogoutCache logoutCache, IUserJwtRefreshRepository userJwtRefreshRepository, IUnitOfWork unitOfWork)
	{
		_config = config;
		_logoutCache = logoutCache;
		_userJwtRefreshRepository = userJwtRefreshRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<JwtTokenDto> GenerateTokenAsync(UserWithPersonalDataDto user)
	{
		//make claims
		var claims = new List<Claim>
		{
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

			new Claim(JwtRegisteredClaimNames.Sub, user.Id),
			//new Claim(JwtRegisteredClaimNames.Email, user.Email!),
			new Claim("Name", user.Name ?? string.Empty),
			new Claim("Surname", user.Surname ?? string.Empty),
			//new Claim("Region", user.SubRegion?.ToString() ?? user.Region?.Id.ToString() ?? string.Empty),
			//new Claim("SubRegion", user.SubRegion?.ToString() ?? string.Empty),
			//new Claim("BanInfo", user.BanInfo?.ToString() ?? string.Empty),
		};
		foreach (var role in user.Roles) claims.Add(new Claim(ClaimTypes.Role, role));
		//make token
		var res = new JwtTokenDto { Token = MakeTokenWithClaims(claims), RefreshToken = GenerateRefreshToken() };
		return res;
	}
	public string GenerateRefreshToken()
	{
		var randomNumber = new byte[32];
		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomNumber);
		return Convert.ToBase64String(randomNumber);
	}

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
			Expires = DateTime.UtcNow.AddHours(double.Parse(_config["Cache:BlackListTimeSpanHour"])),
			SigningCredentials = creds
		};

		var token = tokenHandler.CreateToken(tokenDesc);
		return tokenHandler.WriteToken(token);
	}

	public async Task BlockTokenAsync(string userId, string tokenId)
	{
		_logoutCache.Logout(userId, tokenId);
		_userJwtRefreshRepository.Delete(userId, tokenId);
		await _unitOfWork.SaveChangesAsync(new CancellationToken());
	}

	public Task RefreshTokenAsync(string userId, string tokenId, string refreshToken)
	{
		throw new NotImplementedException();
	}
}
