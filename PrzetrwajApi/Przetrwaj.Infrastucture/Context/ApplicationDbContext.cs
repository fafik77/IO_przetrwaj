using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Przetrwaj.Infrastucture.Context;

public class ApplicationDbContext : DbContext
{



	public ApplicationDbContext(DbContextOptions options) : base(options)
	{
	}

	protected ApplicationDbContext()
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);
	}
}
