using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions._base;

namespace Przetrwaj.Domain.Exceptions.Posts;

public class AlreadyVotedException : AlreadyExistsException<Vote>
{
	public AlreadyVotedException(string identity) : base(identity) { }
}
