using Microsoft.AspNetCore.Http;
using Przetrwaj.Application.Common.Interfaces;
using System.Security.Claims;

namespace Przetrwaj.Presentation.Services;


public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;
    public string? UserId =>
        User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User?.FindFirstValue("sub")
        ?? User?.FindFirstValue("id");
    public string? Email =>
        User?.FindFirstValue(ClaimTypes.Email)
        ?? User?.FindFirstValue("email");
    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;
    public IReadOnlyList<string> Roles =>
        User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        ?? (IReadOnlyList<string>)Array.Empty<string>();
    public bool IsInRole(string role) =>
        User?.IsInRole(role) ?? false;
    public string? GetClaimValue(string claimType) =>
        User?.FindFirstValue(claimType);
}
