using Przetrwaj.CommonLibrary.Models;
using System.Reflection;

namespace Przetrwaj.CommonLibrary.Handlers;

public class GetAppVersionCommonHandler
{
    public static Task<AppVersionDto> GetVersionAsync(CancellationToken cancellationToken = default)
    {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        var assemblyName = assembly.GetName();

        var informationalVersionAttr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        var informationalVersion = informationalVersionAttr?.InformationalVersion;
        string version = assemblyName.Version?.ToString() ?? "1.0.0.0";

        if (!string.IsNullOrEmpty(informationalVersion))
        {
            var parts = informationalVersion.Split('+');
            if (parts.Length > 0 && !string.IsNullOrWhiteSpace(parts[0]))
                version = parts[0];
        }

        DateTime? buildDate = null;
        try
        {
            if (!string.IsNullOrEmpty(assembly.Location) && File.Exists(assembly.Location))
                buildDate = File.GetLastWriteTimeUtc(assembly.Location);
        }
        catch
        {
            // Ignore if file access is restricted or assembly is in-memory
        }

        var result = new AppVersionDto
        {
            Version = version,
            BuildDate = buildDate,
        };

        return Task.FromResult(result);
    }
}
