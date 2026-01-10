using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.Users;

public class GetModeratorPendingQuery : IQuery<IEnumerable<ModeratorPendingStatus>>
{ }
