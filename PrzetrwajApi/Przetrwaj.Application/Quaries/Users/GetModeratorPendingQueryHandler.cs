using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.Users;

public class GetModeratorPendingQueryHandler : IQueryHandler<GetModeratorPendingQuery, IEnumerable<ModeratorPendingStatus>>
{
	private readonly IUserRepository _userRepository;

	public GetModeratorPendingQueryHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<IEnumerable<ModeratorPendingStatus>> Handle(GetModeratorPendingQuery request, CancellationToken cancellationToken)
	{
		var res = await _userRepository.GetModPendingUsersROAsync(cancellationToken);
		return res;
	}
}
