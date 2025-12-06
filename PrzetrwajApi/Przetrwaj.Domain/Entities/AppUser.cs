using Microsoft.AspNet.Identity.EntityFramework;

namespace Przetrwaj.Domain.Entities;

public class AppUser : IdentityUser
{
	public string? Name { get; set; }
	public string? Surname { get; set; }
}
