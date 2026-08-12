using Microsoft.AspNetCore.Components.Forms;

namespace PrzetrwajPL.Components.Pages.Components;

/// <summary>
/// This class stores the user file in memory as it can be only read once! 
/// (and we need to read it n+1 times : where 'n' is the amount of page reloads)
/// </summary>
public class InMemoryBrowserFile : IBrowserFile
{
	private readonly byte[] _bytes;

	public string Name { get; }
	public DateTimeOffset LastModified { get; }
	public long Size { get; }
	public string ContentType { get; }

	public InMemoryBrowserFile(string name, DateTimeOffset lastModified, long size, string contentType, byte[] bytes)
	{
		Name = name;
		LastModified = lastModified;
		Size = size;
		ContentType = contentType;
		_bytes = bytes;
	}

	public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
	{
		return new MemoryStream(_bytes);
	}
}
