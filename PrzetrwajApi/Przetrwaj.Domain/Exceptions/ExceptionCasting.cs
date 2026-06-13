using Microsoft.AspNetCore.Mvc.ModelBinding;
using Przetrwaj.Domain.Exceptions._base;
using System.Net;
using System.Text.Json.Serialization;

namespace Przetrwaj.Domain.Exceptions;

public class ExceptionCasting
{
	public ExceptionCasting()
	{
		Status = string.Empty;
		Error = new ErrorDetails { };
	}
	protected ExceptionCasting(ExceptionCasting other)
	{
		this.Status = other.Status;
		this.StatusCodeEnum = other.StatusCodeEnum;
		this.Error = other.Error;
		this.Timestamp = other.Timestamp;
	}

	public string Status { get; set; }
	[JsonIgnore]
	public HttpStatusCode StatusCodeEnum { get; set; }
	public int StatusCode { get => (int)StatusCodeEnum; }
	public ErrorDetails? Error { get; set; }
	public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

	public static explicit operator ExceptionCasting(BaseException exception)
	{
		return new ExceptionCasting
		{
			StatusCodeEnum = exception.HttpStatusCode,
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
			StatusCodeEnum = HttpStatusCode.BadRequest,
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


public record ErrorDetails
{
	public string Code { get; set; } = string.Empty;
	public string Message { get; set; } = string.Empty;
}