using Microsoft.AspNetCore.Mvc;

namespace Przetrwaj.Application.Helpers;

public class FromAuthorizationHeader : FromHeaderAttribute
{
	public FromAuthorizationHeader() => Name = "Authorization";
}