using System.Security.Claims;

namespace Przetrwaj.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    IReadOnlyList<Claim> Roles { get; }
    bool IsInRole(string role);
    string? GetClaimValue(string claimType);
}
