using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Infrastucture.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Przetrwaj.Infrastucture.Repositories;

public class UserRepository : IUserRepository
{
	private readonly ApplicationDbContext _dbContext;
	public UserRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

}
