using Microsoft.AspNetCore.Hosting;
using Przetrwaj.Domain.Abstractions;

namespace Przetrwaj.Infrastucture.Repositories;

public class AttachmentRepository : IAttachmentRepository
{
	private readonly IWebHostEnvironment _webHostEnvironment;

	public AttachmentRepository(IWebHostEnvironment webHostEnvironment)
	{
		_webHostEnvironment = webHostEnvironment;
	}

	public async Task<bool> SaveAttachmentAsync(Stream FileData, string fileName, CancellationToken cancellationToken)
	{
		string attachmentsPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Attachments");
		string filePath = Path.Combine(attachmentsPath, fileName);
		if (File.Exists(filePath)) return false;
		try
		{
			using (var fs = File.Create(filePath))
			{
				FileData.Seek(0, SeekOrigin.Begin);
				await FileData.CopyToAsync(fs, cancellationToken);
				await fs.FlushAsync(cancellationToken); //just because
			}
		}
		catch (Exception)
		{
			if (File.Exists(filePath))
				File.Delete(filePath); //clean-up the file, there was an error or it was cancelled
			throw;
		}
		return true;
	}
}
