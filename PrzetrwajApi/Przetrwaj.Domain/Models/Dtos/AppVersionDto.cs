using System.Text.Json.Serialization;

namespace Przetrwaj.Domain.Models.Dtos;

public record AppVersionDto
{
    public string Version { get; init; } = string.Empty;

    [JsonIgnore]
    public string? InformationalVersion { get; init; }

    [JsonIgnore]
    public string? CommitHash { get; init; }

    public DateTime? BuildDate { get; init; }

    [JsonIgnore]
    public string? Environment { get; init; }
}
