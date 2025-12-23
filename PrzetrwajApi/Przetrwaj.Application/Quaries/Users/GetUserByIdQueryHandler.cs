using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions.Users;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.Users;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserGeneralDto>
{
	private readonly IUserRepository _userRepository;

	public GetUserByIdQueryHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}


	public async Task<UserGeneralDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
	{
		var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
		if (user is null) throw new UserNotFoundException(request.UserId);
		return (UserGeneralDto)user;
	}
}
