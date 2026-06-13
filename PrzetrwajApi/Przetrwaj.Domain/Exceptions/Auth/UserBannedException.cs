using Microsoft.AspNetCore.Http;
using Przetrwaj.Domain.Exceptions._base;
using Przetrwaj.Domain.Models;
using System.Net;

namespace Przetrwaj.Domain.Exceptions.Auth;

public class UserBannedException(string msg, BanInfo banInfo) : BaseException(msg)
{
	public override HttpStatusCode HttpStatusCode => (HttpStatusCode)StatusCodes.Status418ImATeapot;
	public BanInfo BanInfo { get; } = banInfo;
}
