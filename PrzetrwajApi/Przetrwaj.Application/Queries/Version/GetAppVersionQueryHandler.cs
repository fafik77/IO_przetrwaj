using System.Reflection;
using Microsoft.Extensions.Hosting;
using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Queries.Version;

public class GetAppVersionQueryHandler : IQueryHandler<GetAppVersionQuery, AppVersionDto>
{
	private readonly IHostEnvironment? _environment;

	public GetAppVersionQueryHandler(IHostEnvironment? environment = null)
	{
		_environment = environment;
	}

	public Task<AppVersionDto> Handle(GetAppVersionQuery request, CancellationToken cancellationToken)
	{
		var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
		var assemblyName = assembly.GetName();

		var informationalVersionAttr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
		var informationalVersion = informationalVersionAttr?.InformationalVersion;

		string? commitHash = null;
		string version = assemblyName.Version?.ToString() ?? "1.0.0.0";

		if (!string.IsNullOrEmpty(informationalVersion))
		{
			var parts = informationalVersion.Split('+');
			if (parts.Length > 0 && !string.IsNullOrWhiteSpace(parts[0]))
			{
				version = parts[0];
			}
			if (parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]))
			{
				commitHash = parts[1];
			}
		}

		DateTime? buildDate = null;
		try
		{
			if (!string.IsNullOrEmpty(assembly.Location) && File.Exists(assembly.Location))
			{
				buildDate = File.GetLastWriteTimeUtc(assembly.Location);
			}
		}
		catch
		{
			// Ignore if file access is restricted or assembly is in-memory
		}

		var env = _environment?.EnvironmentName 
			?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
			?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") 
			?? "Production";

		var result = new AppVersionDto
		{
			Version = version,
			InformationalVersion = informationalVersion,
			CommitHash = commitHash,
			BuildDate = buildDate,
			Environment = env
		};

		return Task.FromResult(result);
	}
}
