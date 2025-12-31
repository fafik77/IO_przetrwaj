using Microsoft.AspNetCore.Http;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions._base;
using Przetrwaj.Domain.Models.Dtos;
using System.Net;

namespace Przetrwaj.Domain.Exceptions.Auth;

public class UserBannedException(string msg, UserWithPersonalDataDto user) : BaseException(msg)
{
	public override HttpStatusCode HttpStatusCode => (HttpStatusCode)StatusCodes.Status418ImATeapot;
	public UserWithPersonalDataDto User { get; } = user;
}
