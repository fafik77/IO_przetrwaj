using System.Net;

namespace Przetrwaj.CommonLibrary.Models;


public class ExceptionCasting
{
	public required string Status { get; set; }
	public int StatusCode { get; set; }
	public HttpStatusCode StatusCodeEnum => (HttpStatusCode)StatusCode;
	public required ErrorDetails? Error { get; set; }
	public DateTimeOffset Timestamp { get; set; }
}


public class ErrorDetails
{
	public required string Code { get; set; }
	public required string Message { get; set; }
}
