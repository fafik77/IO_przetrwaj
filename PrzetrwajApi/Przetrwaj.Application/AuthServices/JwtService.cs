using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Przetrwaj.Application.Settings;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Przetrwaj.Application.AuthServices;

public class JwtService : IJwtService
{
	private readonly ILogoutCache _logoutCache;
	private readonly IUserJwtRefreshRepository _userJwtRefreshRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IUserRepository _userRepository;
	private readonly UserManager<AppUser> _userManager;
	private readonly JwtSettings _jwtSettings;

	public JwtService(IOptions<JwtSettings> options, ILogoutCache logoutCache, IUserJwtRefreshRepository userJwtRefreshRepository, IUnitOfWork unitOfWork, IUserRepository userRepository, UserManager<AppUser> userManager)
	{
		_jwtSettings = options.Value;
		_logoutCache = logoutCache;
		_userJwtRefreshRepository = userJwtRefreshRepository;
		_unitOfWork = unitOfWork;
		_userRepository = userRepository;
		_userManager = userManager;
	}

	public async Task<JwtTokenDto> GenerateTokenAsync(UserWithPersonalDataDto user, CancellationToken ct)
	{
		//make claims
		var (tokens, userJwt) = GenerateTokens(user, Guid.NewGuid().ToString());
		userJwt.UsesLeft = 5;
		await _userJwtRefreshRepository.AddAsync(userJwt, ct);
		await _unitOfWork.SaveChangesAsync(ct);
		return tokens;
	}
	public async Task<JwtTokenDto?> RefreshTokenAsync(string userId, string tokenId, string refreshToken, CancellationToken ct)
	{
		var res = await _userJwtRefreshRepository.GetByIdAsync(userId, tokenId, ct);
		if (res is null) return null;
		if (res.RefreshToken != refreshToken) return null;
		if (res.ValidTill <= DateTimeOffset.Now)
			return null;
		//if(--res.UsesLeft == 0)
		//{	//some logic to make it secure ???
		//}
		var user = await _userRepository.GetByIdAsync(userId, ct);
		if (user is null) return null;
		var userDto = (UserWithPersonalDataDto)user;
		var roles = await _userManager.GetRolesAsync(user);
		userDto.Roles = roles;
		//make claims
		var (tokens, userJwt) = GenerateTokens(userDto, res.Jwi);
		res.RefreshToken = userJwt.RefreshToken;
		//invalidate old JWT Jti
		_logoutCache.Logout(userId, tokenId);
		_userJwtRefreshRepository.Update(res);
		await _unitOfWork.SaveChangesAsync(ct);
		return tokens;
	}

	public async Task BlockTokenAsync(string userId, string tokenId, CancellationToken ct)
	{
		_logoutCache.Logout(userId, tokenId);
		await _userJwtRefreshRepository.DeleteAsync(userId, tokenId, ct);
		await _unitOfWork.SaveChangesAsync(ct);
	}
	public async Task BlockAllTokenAsync(string userId, CancellationToken ct)
	{
		var tokens = await _userJwtRefreshRepository.GetByIdAsync(userId, ct);
		foreach (var token in tokens)
		{
			_logoutCache.Logout(userId, token.Jwi);
		}
		await _userJwtRefreshRepository.DeleteAllAsync(userId, ct);
		await _unitOfWork.SaveChangesAsync(ct);
	}





	private (JwtTokenDto tokens, UserJwtRefresh userJwt) GenerateTokens(UserWithPersonalDataDto user, string Jti)
	{
		//make claims
		var claims = new List<Claim>
		{
			new Claim(JwtRegisteredClaimNames.Jti, Jti),

			new Claim(JwtRegisteredClaimNames.Sub, user.Id),
			new Claim("Name", user.Name ?? string.Empty),
			new Claim("Surname", user.Surname ?? string.Empty),
		};
		foreach (var role in user.Roles) claims.Add(new Claim(ClaimTypes.Role, role));
		//make token
		var res = new JwtTokenDto { Token = MakeTokenWithClaims(claims), RefreshToken = GenerateRefreshToken() };
		UserJwtRefresh userJwt = new UserJwtRefresh
		{
			UserId = user.Id,
			Jwi = Jti,
			RefreshToken = res.RefreshToken,
			ValidTill = DateTimeOffset.UtcNow.AddHours(_jwtSettings.RefreshTokenValidHours),
		};
		return (res, userJwt);
	}
	private static string GenerateRefreshToken()
	{
		var randomNumber = new byte[32];
		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomNumber);
		return Convert.ToBase64String(randomNumber);
	}

	private string MakeTokenWithClaims(List<Claim> claims)
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

		var tokenHandler = new JwtSecurityTokenHandler();
		var tokenDesc = new SecurityTokenDescriptor
		{
			Issuer = _jwtSettings.Issuer,
			Audience = _jwtSettings.Audience,
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.UtcNow.AddHours(_jwtSettings.ValidHours),
			SigningCredentials = creds
		};

		var token = tokenHandler.CreateToken(tokenDesc);
		return tokenHandler.WriteToken(token);
	}

}
