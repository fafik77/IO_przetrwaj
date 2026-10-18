using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Services;

public interface ISignInWithService
{
    Task<JwtTokenDto> SignIn(LoginWithExternalProvider info, CancellationToken cancellationToken);
}

public interface ISignInWithGoogleService : ISignInWithService;
