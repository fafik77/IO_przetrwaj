using Przetrwaj.Domain.Exceptions;
using System.Net;

namespace Przetrwaj.Application.Commands.Regions;

public class UpdateRegionBoundsResults
{
	public bool Success { get; set; } = true;
	public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
	public ErrorDetails? Error { get; set; }
	public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
	public short WojCount { get; set; }
	public short PowCount { get; set; }
	public short GmiCount { get; set; }
	//public List<UpdateTercRegionResult> Results { get; set; } = [];
}