using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Domain.Exceptions;

public class AlreadyVotedException : AlreadyExistsException<Vote>
{
    public VoteDto Vote { get; }

    public AlreadyVotedException(string identity, bool? existingIsUpvote = null) : base(identity)
    {
        Vote = new VoteDto(existingIsUpvote);
    }
}
