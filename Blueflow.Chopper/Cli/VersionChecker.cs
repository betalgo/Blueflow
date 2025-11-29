using System.Net.Http.Json;
using System.Reflection;
using Spectre.Console;

namespace Blueflow.Chopper.Cli;

internal sealed class VersionChecker
{
    private const string NugetApiUrl = "https://api.nuget.org/v3-flatcontainer/{0}/index.json";
    private readonly string _packageId;
    private readonly Version _currentVersion;

    public VersionChecker(string packageId, Version currentVersion)
    {
        _packageId = packageId;
        _currentVersion = currentVersion;
    }

    public async Task<string?> CheckForUpdateAsync()
    {
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(3); // Short timeout to not hang if network is slow

            var url = string.Format(NugetApiUrl, _packageId.ToLowerInvariant());
            var response = await client.GetFromJsonAsync<NugetResponse>(url);

            if (response?.Versions == null || response.Versions.Count == 0)
            {
                return null;
            }

            var latestVersion = response.Versions
                .Select(v => Version.TryParse(v, out var parsed) ? parsed : null)
                .Where(v => v != null)
                .Max();

            if (latestVersion != null && latestVersion > _currentVersion)
            {
                return latestVersion.ToString();
            }
        }
        catch
        {
            // Ignore network errors or parsing issues
        }

        return null;
    }

    private class NugetResponse
    {
        public List<string>? Versions { get; set; }
    }
}

