using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using System.Net;
using System.Text.Json.Serialization;

namespace Przetrwaj.Application.Commands.Regions;

public record UpdateTercRegionResult
{
	public RegionPrecision Type { get; set; }
	public required string Status { get; set; }
	public required IRegionInfo Region { get; set; }
	public string? OldName { get; set; }
}
public class UpdateTercRegionsResults
{
	public bool Success { get; set; } = true;
	[JsonIgnore]
	public HttpStatusCode StatusCodeEnum { get; set; } = HttpStatusCode.OK;
	public int StatusCode { get => (int)StatusCodeEnum; }
	public ErrorDetails? Error { get; set; }
	public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
	public short WojCount { get; set; }
	public short PowCount { get; set; }
	public short GmiCount { get; set; }
	public List<UpdateTercRegionResult> Results { get; set; } = [];
}
