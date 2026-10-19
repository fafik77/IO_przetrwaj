using Przetrwaj.CommonLibrary.Consts;

namespace Przetrwaj.CommonLibrary.Models;

public class AttachmentDto
{
    public string? AlternateDescription { get; set; }
    public required string FileURL { get; set; }

    public string ExposedFileURL => string.IsNullOrWhiteSpace(FileURL) ? "" : $"{ApiPaths.PrzetrwajApiExternalUri}{FileURL}";
}
