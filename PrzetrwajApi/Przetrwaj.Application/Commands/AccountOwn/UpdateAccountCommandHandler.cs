using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions.Users;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.AccountOwn;

public class UpdateAccountCommandHandler : ICommandHandler<UpdateAccountInternallCommand, UserWithPersonalDataDto>
{
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateAccountCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}


	public async Task<UserWithPersonalDataDto> Handle(UpdateAccountInternallCommand request, CancellationToken cancellationToken)
	{
		var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
		if (user == null) throw new UserNotFoundException(request.UserId);
		if (request.IdRegion is not null) user.IdRegion = (int)request.IdRegion;
		if (request.Name is not null) user.Name = request.Name;
		if (request.Surname is not null) user.Surname = request.Surname;
		if (request.Email is not null) user.Email = request.Email;

		await _unitOfWork.SaveChangesAsync(cancellationToken); //this line can throw on email

		return (UserWithPersonalDataDto)user;
	}
}
