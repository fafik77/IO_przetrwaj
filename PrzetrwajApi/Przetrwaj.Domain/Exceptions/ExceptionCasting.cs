using Microsoft.AspNetCore.Mvc.ModelBinding;
using Przetrwaj.Domain.Exceptions._base;
using System.Net;

namespace Przetrwaj.Domain.Exceptions;

public class ExceptionCasting
{
	public required string Status { get; set; }
	public HttpStatusCode StatusCode { get; set; }
	public required ErrorDetails Error { get; set; }
	public DateTimeOffset Timestamp { get; set; }

	public static explicit operator ExceptionCasting(BaseException exception)
	{
		return new ExceptionCasting
		{
			StatusCode = exception.HttpStatusCode,
			Status = "error",
			Error = new ErrorDetails
			{
				Code = exception.GetType().Name,
				Message = exception.Message,
			},
			Timestamp = DateTimeOffset.UtcNow,
		};
	}

	public static explicit operator ExceptionCasting(ModelStateDictionary exception)
	{
		return new ExceptionCasting
		{
			StatusCode = HttpStatusCode.BadRequest,
			Status = "error",
			Error = new ErrorDetails
			{
				Code = exception.GetType().Name,
				Message = string.Join("\n", exception.Values),
			},
			Timestamp = DateTimeOffset.UtcNow,
		};
	}
}


public class ErrorDetails
{
	public required string Code { get; set; }
	public required string Message { get; set; }
}