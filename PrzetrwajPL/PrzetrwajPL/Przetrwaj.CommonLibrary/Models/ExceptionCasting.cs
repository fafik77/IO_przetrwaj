using System.Net;

namespace PrzetrwajPL.Models;


public class ExceptionCasting
{
	public required string Status { get; set; }
	public HttpStatusCode StatusCode { get; set; }
	public required ErrorDetails Error { get; set; }
	public DateTimeOffset Timestamp { get; set; }
}


public class ErrorDetails
{
	public required string Code { get; set; }
	public required string Message { get; set; }
}
