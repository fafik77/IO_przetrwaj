namespace Przetrwaj.CommonLibrary.Models;

public record AppVersionDto
{
    public string Version { get; init; } = string.Empty;
    public DateTime? BuildDate { get; init; }
}

public record AppVersionDateDto
{
    public string Version { get; init; } = string.Empty;
    public DateOnly? BuildDate { get; init; }
    public AppVersionDateDto(AppVersionDto appVersionDto)
    {
        this.Version = appVersionDto.Version;
        this.BuildDate = appVersionDto.BuildDate == null ? null : DateOnly.FromDateTime(appVersionDto.BuildDate.Value);
    }
    public AppVersionDateDto()
    { }
}
