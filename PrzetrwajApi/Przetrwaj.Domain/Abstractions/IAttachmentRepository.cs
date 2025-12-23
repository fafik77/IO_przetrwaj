namespace Przetrwaj.Domain.Abstractions;

public interface IAttachmentRepository
{
	/// <summary>
	/// Saves the Attachment on file system
	/// </summary>
	/// <param name="FileData">Attachment file to save</param>
	/// <param name="fileName">Attachment file name with extension</param>
	/// <param name="cancellationToken">cancellationToken</param>
	/// <returns>bool: successful save</returns>
	Task<bool> SaveAttachmentAsync(Stream FileData, string fileName, CancellationToken cancellationToken);
}
